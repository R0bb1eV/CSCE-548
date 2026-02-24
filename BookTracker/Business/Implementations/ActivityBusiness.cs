using BookTracker.Business.Interfaces;
using System;
using System.Collections.Generic;

namespace BookTracker.Business.Implementations
{
    public class ActivityBusiness : IActivityBusiness
    {
        private readonly DataProvider _dataProvider;

        public ActivityBusiness(DataProvider dataProvider)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        public void AddActivity(Activity activity)
        {
            _dataProvider.CreateActivity(activity);
        }

        public List<Activity> GetAllActivities()
        {
            return _dataProvider.ReadAllActivities();
        }

        public Activity? GetActivityById(int id)
        {
            return _dataProvider.ReadActivityById(id);
        }

        public void UpdateActivity(Activity activity)
        {
            _dataProvider.UpdateActivity(activity);
        }

        public void RemoveActivity(int id)
        {
            _dataProvider.DeleteActivity(id);
        }
    }
}
