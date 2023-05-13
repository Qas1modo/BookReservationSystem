using BL.DTOs;
using BL.DTOs.BasicDtos;
using DAL.Enums;
using DAL.Models;

namespace BL.Services.ReservationServ
{
	public interface IReservationService
	{
		void CreateReservation(ReservationDto rentDto,
			RentState state = RentState.Reserved,
			bool commit = false);

		Task<bool> ChangeState(int reservationId, RentState newState,
			int userId, Reservation? reservation = null, bool commit = false);

		Task CancelReservation(int reservationId);

		Task<QueryResultDto<ReservationDetailDto>> ShowReservations(int userId,
			int pageNumber,
			RentState state);
	}
}
