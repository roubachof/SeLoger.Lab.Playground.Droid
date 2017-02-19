﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MetroLog;

using SeLoger.Lab.Playground.Core.Services;
using SeLoger.Lab.Playground.Core.ViewModels.Extensions;

using WeakEvent;

namespace SeLoger.Lab.Playground.Core.ViewModels
{
    public enum DisplayState
    {
        NotStarted = 0,
        Loading,
        Result,
        Error,
    }

    public enum ErrorType
    {
        None = 0,
        Communication,
        Unhandled,
        NoResults 
    }

    public struct ViewModelState
    {
        public ViewModelState(DisplayState state, ErrorType error = ErrorType.None, bool isRefreshed = false)
        {
            Display = state;
            Error = error;
            IsRefreshed = isRefreshed;
        }

        public ViewModelState(DisplayState state, bool isRefreshed)
            : this(state, ErrorType.None, isRefreshed)
        {
        }

        public DisplayState Display { get; }

        public bool IsRefreshed { get; }

        public ErrorType Error { get; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("ViewModelState (");
            sb.Append($" {Enum.GetName(typeof(DisplayState), Display)}");
            sb.Append($" {Enum.GetName(typeof(ErrorType), Error)}");

            string refreshState = IsRefreshed ? "has been refreshed" : "has not been refreshed";
            sb.Append($" {refreshState}");
            sb.Append(")");
            return sb.ToString();
        }
    }

    public enum ViewModelState2
    {
        NotStarted = 0,
        Loading,
        LoadingMore,
        SuccessfullyLoaded,
        SuccessfullyLoadedNoResults,
        CommunicationError,
        UnhandledError
    }

    public class SillyPeopleViewModel
    {
        private static readonly ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger(nameof(SillyPeopleViewModel));

        private const int PAGE_SIZE = 50;

        private const int MAX_ITEMS = 300;

        private readonly WeakEventSource<EventArgs> _taskCompletedSource = new WeakEventSource<EventArgs>();

        private readonly ISillyFrontService _sillyFrontService;

        public SillyPeopleViewModel(ISillyFrontService sillyFrontService)
        {
            _sillyFrontService = sillyFrontService;

            Paginator = new Paginator<SillyDudeItemViewModel>(PAGE_SIZE, MAX_ITEMS, PaginatorDataSource, OnPaginatorTaskCompleted);
        }
        
        public event EventHandler<EventArgs> TaskCompleted
        {
            add { _taskCompletedSource.Subscribe(value); }
            remove { _taskCompletedSource.Unsubscribe(value); }
        }

        public string Title => $"{Paginator.LoadedCount} silly guys loaded";

        public Paginator<SillyDudeItemViewModel> Paginator { get; }

        public ViewModelState GetState()
        {
            return Paginator.ToViewModelState();
        }

        public void Load()
        {
            Log.Info("Loading view model");
            Paginator.LoadPage(1);
        }
        
        private async Task<PageResult<SillyDudeItemViewModel>> PaginatorDataSource(int pageNumber, int pageSize)
        {
            var modelPageResult = await _sillyFrontService.GetSillyPeoplePage(pageNumber, pageSize);
            return new PageResult<SillyDudeItemViewModel>(
                modelPageResult.TotalCount,
                modelPageResult.Items.Select(model => new SillyDudeItemViewModel(model)).ToList());
        }
        
        private void OnPaginatorTaskCompleted()
        {
            Log.Info("OnPaginatorTaskCompleted");
            _taskCompletedSource.Raise(this, EventArgs.Empty);
        }
    }
}
