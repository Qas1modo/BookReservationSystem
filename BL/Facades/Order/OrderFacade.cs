using AutoMapper;
using BL.DTOs.BasicDtos;
using BL.Services.CartItemServ;
using BL.Services.ReservationServ;
using BL.Services.StockServ;
using DAL.Enums;
using DAL.Models;
using Infrastructure.Query;
using Infrastructure.UnitOfWork;

namespace BL.Facades.OrderFac
{
	public class OrderFacade : IOrderFacade
	{
		private readonly IStockService stockService;
		private readonly ICartItemService cartService;
		private readonly IReservationService rentService;
		private readonly IUoWReservation uow;
		private readonly IMapper mapper;
		private readonly IQuery<Reservation> query;

		public OrderFacade(IStockService stockService,
			ICartItemService cartService,
			IReservationService rentService,
			IUoWReservation uow,
			IMapper mapper,
			IQuery<Reservation> query)
		{
			this.stockService = stockService;
			this.rentService = rentService;
			this.cartService = cartService;
			this.query = query;
			this.uow = uow;
			this.mapper = mapper;
		}

		public async Task<bool> MakeOrder(int userId)
		{
			User user = await uow.UserRepository.GetByID(userId);
			if (!await UserReservations(userId, user.CartItems.Count))
			{
				return false;
			}
			RentState state = RentState.Reserved;
			foreach (CartItem cartItem in user.CartItems)
			{
				if (cartItem.Book.Deleted)
				{
					return false;
				}
				if (!await stockService.ReserveBookStock(cartItem.BookId))
				{
					state = RentState.Awaiting;
				}
				rentService.CreateReservation(mapper.Map<ReservationDto>(cartItem), state);
			}
			await cartService.EmptyCart(userId, false);
			await uow.CommitAsync();
			return true;
		}

		private async Task<bool> UserReservations(int userId, int reservationCount = 1)
		{
			int userReservations = (await query
				.Where<RentState>(x => x.Equals(RentState.Reserved) || x.Equals(RentState.Awaiting), "State")
				.Where<int>(x => x == userId, "UserId")
				.Execute()).ItemsCount;
			return userReservations + reservationCount < 6;
		}

		public async Task<bool> ReserveBook(int reservationId, int userId)
		{
			Reservation reservation = await uow.ReservationRepository.GetByID(reservationId);
			if (!await UserReservations(userId) || reservation.Book.Deleted)
			{
				return false;
			}
			RentState state = RentState.Reserved;
			if (!await stockService.ReserveBookStock(reservation.BookId))
			{
				state = RentState.Awaiting;
			}
			if (!await rentService.ChangeState(reservationId, state, userId))
			{
				return false;
			}
			await uow.CommitAsync();
			return true;
		}

		private async Task<bool> IsAwaiting(int bookId)
		{
			Reservation? waitingReservation = (await query
			   .Where<RentState>(x => x == RentState.Awaiting, "State")
			   .Where<int>(x => x == bookId, "BookId")
			   .OrderBy<DateTime>("ReservedAt")
			   .Page(1, 1)
			   .Execute()).Items
			   .FirstOrDefault();
			if (waitingReservation != null)
			{
				await rentService.ChangeState(waitingReservation.Id, RentState.Reserved, waitingReservation.UserId);
				return true;
			}
			return false;
		}

		public async Task<bool> ReturnBook(int reservationId,
			int userId,
			RentState newState = RentState.Returned,
			bool commit = true)
		{
			Reservation reservation = await uow.ReservationRepository.GetByID(reservationId);
			if (!await rentService.ChangeState(reservationId, newState, userId, reservation))
			{
				return false;
			}
			if (!await IsAwaiting(reservation.BookId))
			{
				await stockService.BookReturnedStock(reservation.BookId);
			}
			if (commit) await uow.CommitAsync();
			return true;
		}

		public async Task ExpireOldReservations()
		{
			DateTime currentTime = DateTime.Now;
			IEnumerable<Reservation> reservations = (await query
				.Where<RentState>(x => x == RentState.Reserved, "State")
				.Execute()).Items;
			foreach (Reservation reservation in reservations)
			{
				if (DateTime.Now.Subtract(reservation.ReservedAt).Days > 7)
				{
					await ReturnBook(reservation.Id, reservation.UserId, RentState.Expired, false);
				}
			}
			await uow.CommitAsync();
		}
	}
}

