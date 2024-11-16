using FinalProject.Data;
using FinalProject.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Services.Admin
{
    public class ResponseListService
    {
        private readonly QlptContext db;

        public ResponseListService(QlptContext db)
        {
            this.db = db;
        }

        public List<ResponseListVM> GetAllResponseList()
        {
            try
            {
                var responseList = (from response in db.Responses
                                    join roomFeedback in db.RoomFeedbacks on response.RoomFeedbackId equals roomFeedback.RoomFeedbackId
                                    join roomPost in db.RoomPosts on roomFeedback.PostId equals roomPost.PostId
                                    join feedback in db.Feedbacks on roomFeedback.FeedbackId equals feedback.FeedbackId
                                    join user in db.Users on roomPost.UserId equals user.UserId
                                    select new ResponseListVM
                                    {
                                        ResponseId = response.ResponseId,
                                        UserName = user.Fullname,
                                        FeedbackName = feedback.FeedbackName,
                                        Address = roomPost.Address,
                                        ResponseText = response.ResponseText,
                                        ResponseDate = response.ResponseDate
                                    }).ToList();

                return responseList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public ResponseListVM GetResponseById(int responseId)
        {
            try
            {
                var _response = (from response in db.Responses
                                    join roomFeedback in db.RoomFeedbacks on response.RoomFeedbackId equals roomFeedback.RoomFeedbackId
                                    join roomPost in db.RoomPosts on roomFeedback.PostId equals roomPost.PostId
                                    join feedback in db.Feedbacks on roomFeedback.FeedbackId equals feedback.FeedbackId
                                    join user in db.Users on roomPost.UserId equals user.UserId
                                    where response.ResponseId == responseId
                                    select new ResponseListVM
                                    {
                                        ResponseId = response.ResponseId,
                                        UserName = user.Fullname,
                                        FeedbackName = feedback.FeedbackName,
                                        Address = roomPost.Address,
                                        ResponseText = response.ResponseText,
                                        ResponseDate = response.ResponseDate
                                    }).FirstOrDefault();

                return _response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public bool Approve(int responseId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var response = db.Responses
                        .Where(r => r.ResponseId == responseId)
                        .FirstOrDefault();

                    if (response == null)
                    {
                        return false;
                    }

                    var roomFeedbackId = response.RoomFeedbackId;

                    db.Responses.Remove(response);
                    db.SaveChanges();

                    var roomFeedback = db.RoomFeedbacks
                        .Where(rf => rf.RoomFeedbackId == roomFeedbackId)
                        .FirstOrDefault();

                    if (roomFeedback == null)
                    {
                        return false;
                    }

                    var postId = roomFeedback.PostId;
                    var feedbackId = roomFeedback.FeedbackId;

                    var recordsToDelete = db.RoomFeedbacks
                        .Where(rf => rf.PostId == postId && rf.FeedbackId == feedbackId)
                        .ToList();

                    if (recordsToDelete.Any())
                    {
                        db.RoomFeedbacks.RemoveRange(recordsToDelete);
                        db.SaveChanges();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public bool Reject(int responseId)
        {
            try
            {
                var response = db.Responses
                    .Where(r => r.ResponseId == responseId)
                    .FirstOrDefault();

                if (response == null)
                {
                    return false; 
                }

                db.Responses.Remove(response);
                db.SaveChanges();

                return true; 
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
