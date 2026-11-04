using System.ComponentModel.DataAnnotations;
namespace RaceDay.Mvc.Models;
public class LoginVm{[Required,EmailAddress]public string Email{get;set;}="";[Required]public string Password{get;set;}="";}
public class RegisterVm{[Required]public string FirstName{get;set;}="";[Required]public string LastName{get;set;}="";[Required,EmailAddress]public string Email{get;set;}="";[Required,MinLength(8)]public string Password{get;set;}="";[Required]public string Role{get;set;}="Participant";public string? Phone{get;set;}}
public class EventVm{public int EventId{get;set;}[Required]public string Name{get;set;}="";[Required]public string Description{get;set;}="";[Required]public DateTime EventDate{get;set;}=DateTime.Now.AddDays(30);[Required]public string Location{get;set;}="";[Range(.1,9999)]public decimal DistanceKm{get;set;}=5;[Required]public string EventType{get;set;}="Run";}
public class CategoryVm{[Required]public string Name{get;set;}="";[Required]public string CategoryType{get;set;}="Distance";public byte? MinimumAge{get;set;}public byte? MaximumAge{get;set;}public decimal? DistanceKm{get;set;}public decimal EntryFee{get;set;}}
public class ResultVm{public int EnrolmentId{get;set;}public string FinishTime{get;set;}="00:00:00";public int Position{get;set;}=1;}
public class ProfileVm{[Required]public string FirstName{get;set;}="";[Required]public string LastName{get;set;}="";public string? Phone{get;set;}public string? ProfileImageUrl{get;set;}}
public record User(int UserId,string FirstName,string LastName,string Email,string Role,string? Phone,string? ProfileImageUrl,DateTime CreatedAt);
public record EventSummary(int EventId,string Name,DateTime EventDate,string Location,decimal DistanceKm,string EventType,string? BannerImageUrl,int OrganiserId,int EnrolmentCount);
public record Category(int CategoryId,int EventId,string Name,string CategoryType,byte? MinimumAge,byte? MaximumAge,decimal? DistanceKm,decimal EntryFee);
public record EventDetail(int EventId,string Name,string Description,DateTime EventDate,string Location,decimal DistanceKm,string EventType,string? BannerImageUrl,int OrganiserId,string OrganiserName,IReadOnlyList<Category> Categories);
public record Enrolment(int EnrolmentId,int ParticipantId,string ParticipantName,int EventId,string EventName,DateTime EventDate,int CategoryId,string CategoryName,string Status,DateTime EnrolledAt);
public record Result(int ResultId,int EnrolmentId,int EventId,string EventName,DateTime EventDate,string CategoryName,string ParticipantName,TimeSpan FinishTime,int Position,int TotalFinishers);