﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Models.Mini
{
	public class MiniTrainingMaterial : MiniModel {
		public MiniTrainingMaterial(Guid guid, string name) : base(guid, name) {
			Title = name;
		}

		public string Description { get; set; }
		public string ThumbnailUrl { get; set; }
		public string Title { get; }
	}
}