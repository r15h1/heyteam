﻿using HeyTeam.Util;
using System;

namespace HeyTeam.Core {
	public class TrainingMaterial
    {
		public TrainingMaterial(Guid clubId, Guid? trainingMaterialId = null) {
			if (clubId.IsEmpty()) throw new ArgumentNullException();
			ClubId = clubId;
			Guid = trainingMaterialId ?? System.Guid.NewGuid();
		}

		public Guid ClubId { get; }
		public Guid? Guid { get; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string ContentType { get; set; }
		public string ExternalId { get; set; }
		public string Url { get; set; }
		public string ThumbnailUrl { get; set; }
	}
}