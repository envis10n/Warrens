﻿using NetMud.DataAccess;
using NetMud.DataStructure.Base.Entity;
using NetMud.DataStructure.Base.EntityBackingData;
using NetMud.DataStructure.Base.Place;
using NetMud.DataStructure.Base.Supporting;
using NetMud.DataStructure.Base.System;
using NetMud.DataStructure.Behaviors.Automation;
using NetMud.DataStructure.Behaviors.Rendering;
using NetMud.DataStructure.SupportingClasses;
using NetMud.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMud.Data.Game
{
    public class Room : IRoom
    {
        public Room()
        {
            ObjectsInRoom = new EntityContainer<IObject>();
            MobilesInRoom = new EntityContainer<IMobile>();
        }

        public Room(IRoomData room)
        {
            ObjectsInRoom = new EntityContainer<IObject>();
            MobilesInRoom = new EntityContainer<IMobile>();

            //Yes it's its own datatemplate and currentLocation
            DataTemplate = room;

            CurrentLocation = this;

            GetFromWorldOrSpawn();
        }

        public string BirthMark { get; private set; }

        public DateTime Birthdate { get; private set; }

        public string[] Keywords { get; set; }

        public IData DataTemplate { get; private set; }

        public ILocation CurrentLocation { get; set; }

        #region Container
        public EntityContainer<IObject> ObjectsInRoom { get; set; }
        public EntityContainer<IMobile> MobilesInRoom { get; set; }

        public IEnumerable<T> GetContents<T>()
        {
            var implimentedTypes = DataUtility.GetAllImplimentingedTypes(typeof(T));

            var contents = new List<T>();

            if (implimentedTypes.Contains(typeof(IMobile)))
                contents.AddRange(GetContents<T>("mobiles"));
            
            if (implimentedTypes.Contains(typeof(IObject)))
                contents.AddRange(GetContents<T>("objects"));

            return contents;
        }

        public IEnumerable<T> GetContents<T>(string containerName)
        {
            switch (containerName)
            {
                case "mobiles":
                    return MobilesInRoom.EntitiesContained.Select(ent => (T)ent);
                case "objects":
                    return ObjectsInRoom.EntitiesContained.Select(ent => (T)ent);
            }

            return Enumerable.Empty<T>();
        }

        public string MoveTo<T>(T thing)
        {
            return MoveTo<T>(thing, String.Empty);
        }

        public string MoveTo<T>(T thing, string containerName)
        {
            var implimentedTypes = DataUtility.GetAllImplimentingedTypes(typeof(T));

            if (implimentedTypes.Contains(typeof(IObject)))
            {
                var obj = (IObject)thing;

                if (ObjectsInRoom.Contains(obj))
                    return "That is already in the container";

                ObjectsInRoom.Add(obj);
                return String.Empty;
            }
            else if (implimentedTypes.Contains(typeof(IMobile)))
            {
                var mob = (IMobile)thing;

                if (MobilesInRoom.Contains(mob))
                    return "That is already in the container";

                MobilesInRoom.Add(mob);
                return String.Empty;
            }

            return "Invalid type to move to container.";
        }

        public string MoveFrom<T>(T thing)
        {
            return MoveFrom<T>(thing, String.Empty);
        }

        public string MoveFrom<T>(T thing, string containerName)
        {
            var implimentedTypes = DataUtility.GetAllImplimentingedTypes(typeof(T));

            if (implimentedTypes.Contains(typeof(IObject)))
            {
                var obj = (IObject)thing;

                if (!ObjectsInRoom.Contains(obj))
                    return "That is not in the container";

                ObjectsInRoom.Remove(obj);
                return String.Empty;
            }
            if (implimentedTypes.Contains(typeof(IMobile)))
            {
                var mob = (IMobile)thing;

                if (!MobilesInRoom.Contains(mob))
                    return "That is not in the container";

                MobilesInRoom.Remove(mob);
                return String.Empty;
            }

            return "Invalid type to move from container.";
        }
        #endregion

        public IEnumerable<string> RenderToLook()
        {
            var sb = new List<string>();

            sb.Add(string.Format("<span style=\"color: orange\">{0}</span>", DataTemplate.Name));
            sb.Add(string.Empty.PadLeft(DataTemplate.Name.Length, '-'));

            return sb;
        }

        public void GetFromWorldOrSpawn()
        {
            var liveWorld = new LiveCache();

            //Try to see if they are already there
            var me = liveWorld.Get<IRoom>(DataTemplate.ID, typeof(IRoom));

            //Isn't in the world currently
            if (me == default(IRoom))
                SpawnNewInWorld();
            else
            {
                BirthMark = me.BirthMark;
                Keywords = me.Keywords;
                Birthdate = me.Birthdate;
                CurrentLocation = me.CurrentLocation;
                DataTemplate = me.DataTemplate;
            }
        }

        public void SpawnNewInWorld()
        {
            //TODO: will rooms ever be contained by something else?
            SpawnNewInWorld(this);
        }

        public void SpawnNewInWorld(ILocation spawnTo)
        {
            var liveWorld = new LiveCache();
            var roomTemplate = (IRoomData)DataTemplate;

            BirthMark = Birthmarker.GetBirthmark(roomTemplate);
            Keywords = new string[] { roomTemplate.Name.ToLower() };
            Birthdate = DateTime.Now;
            CurrentLocation = spawnTo;

            liveWorld.Add<IRoom>(this);
        }

        public int CompareTo(IEntity other)
        {
            if (other != null)
            {
                try
                {
                    if (other.GetType() != typeof(Room))
                        return -1;

                    if (other.BirthMark.Equals(this.BirthMark))
                        return 1;

                    return 0;
                }
                catch
                {
                    //Minor error logging
                }
            }

            return -99;
        }

        public bool Equals(IEntity other)
        {
            if (other != default(IEntity))
            {
                try
                {
                    return other.GetType() == typeof(Room) && other.BirthMark.Equals(this.BirthMark);
                }
                catch
                {
                    //Minor error logging
                }
            }

            return false;
        }

    }
}