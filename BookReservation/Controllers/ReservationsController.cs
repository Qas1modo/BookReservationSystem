using BL.DTOs;
using BL.Facades.OrderFac;
using BL.Services.ReservationServ;
using DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
	[Route("/reservations/{userId:int}"), Authorize]
	public class ReservationsController : CommonController
	{
		private readonly IReservationService _reservationService;
		private readonly IOrderFacade _orderFacade;

		public ReservationsController(IReservationService reservationService,
			IOrderFacade orderFacade)
		{
			_reservationService = reservationService;
			_orderFacade = orderFacade;
		}

		private async Task<ReservationModel<ReservationDetailDto>> Reservations(int page, RentState state, int userId)
		{
			if (page < 1) page = 1;
			string group = GetGroup();
			userId = GetValidUser(userId, group);
			var result = await _reservationService.ShowReservations(userId, page, state);
			ReservationModel<ReservationDetailDto> model = new()
			{
				CurrentState = state,
				Items = result.Items,
				PageCount = (result.ItemsCount - 1) / result.PageSize + 1,
				PageNumber = result.PageNumber ?? 1,
				Group = group,
				UserId = userId,
			};
			return model;
		}

		[Route("cancel/{id:int}/{page:int?}")]
		[Authorize]
		public async Task<IActionResult> Cancel(int userId, int id, int page = 1)
		{
			if (!CheckPermissions(userId))
			{
				return View("Reservations", await Reservations(page, RentState.Reserved, userId));
			}
			if (!await _orderFacade.ReturnBook(id, userId, RentState.Canceled))
			{
				ModelState.AddModelError("Id", "Invalid operation!");
			};
			return View("Reservations", await Reservations(page, RentState.Reserved, userId));
		}

		[Route("renew/{id:int}/{page:int?}")]
		[Authorize]
		public async Task<IActionResult> Renew(int userId, int id, int page = 1)
		{
			if (CheckPermissions(userId))
			{
				if (!await _orderFacade.ReserveBook(id, userId))
				{
					ModelState.AddModelError("Id", "Cannot have more than 5 reservations!");
				}
			}
			return View("Reservations", await Reservations(page, RentState.Expired, userId));
		}

		[Route("picked/{id:int}/{page:int?}")]
		[Authorize(Roles = "Admin, Employee")]
		public async Task<IActionResult> Picked(int userId, int id, int page = 1)
		{
			if (!await _reservationService.ChangeState(id, RentState.Active, userId, commit: true))
			{
				ModelState.AddModelError("Id", "Invalid operation!");
			}
			return View("Reservations", await Reservations(page, RentState.Reserved, userId));
		}

		[Route("returned/{id:int}/{page:int?}")]
		[Authorize(Roles = "Admin, Employee")]
		public async Task<IActionResult> Returned(int userId, int id, int page = 1)
		{
			if (!await _reservationService.ChangeState(id, RentState.Returned, userId, commit: true))
			{
				ModelState.AddModelError("Id", "Invalid operation!");
			}
			return View("Reservations", await Reservations(page, RentState.Active, userId));
		}

		[HttpGet("reserved/{page:int?}")]
		public async Task<IActionResult> Reserved(int userId, int page = 1)
		{
			return View("Reservations", await Reservations(page, RentState.Reserved, userId));
		}

		[HttpGet("awaiting/{page:int?}")]
		public async Task<IActionResult> Awaiting(int userId, int page = 1)
		{
			return View("Reservations", await Reservations(page, RentState.Awaiting, userId));
		}

		[HttpGet("canceled/{page:int?}")]
		public async Task<IActionResult> Canceled(int userId, int page = 1)
		{
			return View("Reservations", await Reservations(page, RentState.Canceled, userId));
		}

		[HttpGet("active/{page:int?}")]
		public async Task<IActionResult> Active(int userId, int page = 1)
		{
			return View("Reservations", await Reservations(page, RentState.Active, userId));
		}

		[HttpGet("returned/{page:int?}")]
		public async Task<IActionResult> Returned(int userId, int page = 1)
		{
			return View("Reservations", await Reservations(page, RentState.Returned, userId));
		}

		[HttpGet("expired/{page:int?}")]
		public async Task<IActionResult> Expired(int userId, int page = 1)
		{
			return View("Reservations", await Reservations(page, RentState.Expired, userId));
		}

		[HttpGet("{page:int?}")]
		public async Task<IActionResult> Index(int userId, int page = 1)
		{

			return View("Reservations", await Reservations(page, RentState.Reserved, userId));
		}
	}
}
