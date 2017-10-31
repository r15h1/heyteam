using System;
using System.Collections.Generic;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Core.UseCases.Club {
    public class UpdateClubProfileRequest {
        public Guid ClubId { get; set; }

        public Dictionary<UpdatableFields, string> Fields { get; } = new Dictionary<UpdatableFields, string>();
        public void SetFieldValue(UpdatableFields field, string value) {
            if(!value.IsEmpty()) {
                if (Fields.ContainsKey(field))
                    Fields[field] = value;
                else
                    Fields.Add(field, value);
            }
        }

        public void RemoveField(UpdatableFields field) {
            if (Fields.ContainsKey(field))
                Fields.Remove(field);
        }
        
        public enum UpdatableFields {
            NAME,
            URL
        }
    }
}