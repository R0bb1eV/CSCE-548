using System.Collections.Generic;

namespace BookTracker.Business.Interfaces
{
    public interface IActivityBusiness
    {
        void AddActivity(Activity activity);
        List<Activity> GetAllActivities();
        Activity? GetActivityById(int id);
        void UpdateActivity(Activity activity);
        void RemoveActivity(int id);
    }
}
