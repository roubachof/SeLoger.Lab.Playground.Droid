﻿using SeLoger.Lab.Playground.Core.Models;

namespace SeLoger.Lab.Playground.Core.ViewModels
{
    public class SillyDudeItemViewModel
    {
        public SillyDudeItemViewModel(SillyDudeModel model)
        {
            Id = model.Id;
            ImageUrl = model.ImageUrl;
            Name = $"{model.Id}. {model.Name}";
            Role = model.Role;
        }

        public int Id { get; }
        public string ImageUrl { get; }
        public string Name { get; }
        public string Role { get; }
    }
}
