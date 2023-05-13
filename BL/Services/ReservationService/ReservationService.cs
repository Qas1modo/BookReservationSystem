using AutoMapper;
using BL.DTOs;
using BL.DTOs.BasicDtos;
using DAL.Enums;
using DAL.Models;
using Infrastructure.Query;
using Infrastructure.UnitOfWork;

namespace BL.Services.ReservationServ
{
	public class ReservationService : IReservationService
	{
		private readonly IMapper mapper;
		private readonly IUoWReservation uow;
		private readonly IQuery<Reservation> query;

		public ReservationService(IUoWReservation uow,
			IMapper mapper,
			IQuery<Reservation> query)
		{
			this.mapper = mapper;
			this.uow = uow;
			this.query = query;
		}

		public void CreateReservation(ReservationDto rentDto, RentState state = RentState.Reserved, bool commit = false)
		{
			rentDto.State = state;
			rentDto.ReservedAt = DateTime.Now;
			uow.ReservationRepository.Insert(mapper.Map<Reservation>(rentDto));
			if (commit) uow.CommitAsync();
		}

		public async Task CancelReservation(int reservationId)
		{
			Reservation reservation = await uow.ReservationRepository.GetByID(reservationId);
			RentState state = reservation.State;
			if (state == RentState.Awaiting ||
				state == RentState.Reserved ||
				state == RentState.Expired)
			{
				reservation.CanceledAt = DateTime.Now;
				reservation.State = RentState.Canceled;
				uow.ReservationRepository.Update(reservation);
			}
		}


		public async Task<bool> ChangeState(int reservationId, RentState newState,
			int userId = -1, Reservation? reservation = null, bool commit = false)
		{
			reservation ??= await uow.ReservationRepository.GetByID(reservationId);
			if (userId != -1 && userId != reservation.UserId)
			{
				return false;
			}
			switch (newState)
			{
				case RentState.Awaiting:
					if (reservation.State == RentState.Expired)
					{
						reservation.ReservedAt = DateTime.Now;
						break;
					}
					return false;
				case RentState.Expired:
					if (reservation.State == RentState.Reserved)
					{
						reservation.CanceledAt = DateTime.Now;
						break;
					}
					return false;
				case RentState.Canceled:
					if (reservation.State == RentState.Reserved ||
						reservation.State == RentState.Awaiting)
					{
						reservation.CanceledAt = DateTime.Now;
						break;
					}
					return false;
				case RentState.Reserved:
					if (reservation.State == RentState.Expired ||
						reservation.State == RentState.Awaiting)
					{
						reservation.ReservedAt = DateTime.Now;
						reservation.CanceledAt = null;
						break;
					}
					return false;
				case RentState.Returned:
					if (reservation.State == RentState.Active)
					{
						reservation.ReturnedAt = DateTime.Now;
						break;
					}
					return false;
				case RentState.Active:
					if (reservation.State == RentState.Reserved)
					{
						reservation.RentedAt = DateTime.Now;
						break;
					}
					return false;
			}
			reservation.State = newState;
			uow.ReservationRepository.Update(reservation);
			if (commit) await uow.CommitAsync();
			return true;
		}

		public async Task<QueryResultDto<ReservationDetailDto>> ShowReservations(int userId,
			int pageNumber,
			RentState state)
		{
			query.Where<int>(x => x == userId, "UserId");
			query.Where<RentState>(x => x.Equals(state), "State");
			query.OrderBy<DateTime>("ReservedAt", false);
			query.Page(pageNumber, 10);
			var result = await query.Execute();
			return mapper.Map<QueryResult<Reservation>, QueryResultDto<ReservationDetailDto>>(result);
		}
	}
}
