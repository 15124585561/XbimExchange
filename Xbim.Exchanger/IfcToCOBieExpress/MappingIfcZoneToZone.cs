﻿using System.Collections.Generic;
using System.Linq;
using Xbim.COBieLiteUK;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace XbimExchanger.IfcToCOBieExpress
{
    class MappingIfcZoneToZone : XbimMappings<IfcStore, List<Facility>, string, IIfcZone, Zone>
    {
        protected override Zone Mapping(IIfcZone ifcZone, Zone target)
        {
            var helper = ((IfcToCoBieExpressExchanger)Exchanger).Helper;
            target.ExternalEntity = helper.ExternalEntityName(ifcZone);
            target.ExternalId = helper.ExternalEntityIdentity(ifcZone);
            target.AltExternalId = ifcZone.GlobalId;
            target.ExternalSystem = helper.ExternalSystemName(ifcZone);
            target.Description = ifcZone.Description;
            target.Categories = helper.GetCategories(ifcZone);
            if (target.Categories==COBieExpressHelper.UnknownCategory)
             if(!string.IsNullOrWhiteSpace(ifcZone.ObjectType) )
                 target.Categories = new List<Category>(new [] {new Category{Code=ifcZone.ObjectType}} );
            target.CreatedBy = helper.GetCreatedBy(ifcZone);
            target.CreatedOn = helper.GetCreatedOn(ifcZone);
            target.Name = ifcZone.Name;
            //Attributes
            target.Attributes = helper.GetAttributes(ifcZone);
            //Documents
            var docsMappings = Exchanger.GetOrCreateMappings<MappingIfcDocumentSelectToDocument>();
            helper.AddDocuments(docsMappings, target, ifcZone);

            //get spaces in zones
            var spaces = helper.GetSpaces(ifcZone);
            var ifcSpaces = spaces as IList<IIfcSpace> ?? spaces.ToList();
            if (ifcSpaces.Any())
            {
                target.Spaces = new List<SpaceKey>();
                foreach (var space in ifcSpaces)
                {
                    var spaceKey = new SpaceKey { Name = space.Name };
                    target.Spaces.Add(spaceKey);
                }
            }
            //TODO:
            //Space Issues
            
            return target;
        }

        public override Zone CreateTargetObject()
        {
            return new Zone();
        }
    }
}