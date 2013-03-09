/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DDW.Vex
{
	public class SoundInstance : IInstance
	{
		public uint SortOrder { get { return 1; } }

		private uint definitionId;
		public uint DefinitionId { get { return definitionId; } set { definitionId = value; } }

		private uint instanceID;
		public uint InstanceHash { get { return instanceID; } set { instanceID = value; } }

		private uint startTime;
        public uint StartTime { get { return startTime; } set { startTime = value; } }

        public int Depth { get; set; }
        public string Name { get; set; }
        public uint ParentDefinitionId { get; set; }
        public Point RotationCenter { get { return Point.Empty; } set { } }
        [XmlIgnore]
        public bool HasSaveableChanges { get; set; }

        private List<Transform> transformations = new List<Transform>();
        public List<Transform> Transformations { get { return transformations; } }
		
		public string Path;

		public SoundInstance(string path, uint id)
		{
			this.definitionId = id;
			this.Path = path;
		}

		public int CompareTo(Object o)
		{
			int result = 0;
			if (o is SoundInstance)
			{
				SoundInstance inst = (SoundInstance)o;
				if (this.StartTime != inst.StartTime)
				{
					result = ((int)this.StartTime) > ((int)inst.StartTime) ? 1 : -1;
				}
			}
			else if (o is IInstance)
			{
				IInstance inst = (IInstance)o;
				result = ((int)this.SortOrder) > ((int)inst.SortOrder) ? 1 : -1;
			}
			else
			{
				throw new ArgumentException("Objects being compared are not of the same type");
			}
			return result;
		}

        public Transform GetTransformAtTime(uint time)
        {
            Transform result = null;
            int index = transformations.FindIndex(item => (item.StartTime <= time) && (item.EndTime >= time));
            if (index > -1)
            {
                result = transformations[index];
            }
            return result;
        }
	}
}
