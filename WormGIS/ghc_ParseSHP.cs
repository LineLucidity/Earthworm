﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using DotSpatial.Data;
using DotSpatial.Serialization;
using Grasshopper;
using Grasshopper.Kernel.Data;


// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace WormGIS
{
    public class ghc_ParseSHP : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ghc_ParseSHP()
          : base("ParseShapefile", "ParseSHP",
              "Parses a shapefile into the Grasshopper environment",
              "WormGIS", "Utilities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
            // Compulsory inputs
            pManager.AddTextParameter("Path", "P", "File path or directory of shapefile as string.", GH_ParamAccess.item);

            // Optional inputs
            Params.Input[
            pManager.AddVectorParameter("Vector", "V", "Option translation vector", GH_ParamAccess.item)
            ].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            
            pManager.AddTextParameter("Key", "K ", "Keys of feature attributes", GH_ParamAccess.tree);
            pManager.AddTextParameter("Value", "V ", "Values of feature attributes", GH_ParamAccess.tree);
            pManager.AddPointParameter("Points", "P", "feature geometry as point", GH_ParamAccess.tree);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = "";
            if (!DA.GetData("Path", ref path)) return;

            Vector3d vec = new Vector3d(0, 0, 0);
            if (!DA.GetData("Vector", ref vec)) return;

            // Open shapefile from path
            Shapefile shp = Shapefile.OpenFile(path);

            
            
            // Setup output
            DataTree<Point3d> ptTree = new DataTree<Point3d>();
            DataTree<string> keysTree = new DataTree<string>();
            DataTree<string> valsTree = new DataTree<string>();


            //Read features in the shapefile 
            int pathCount = 0;
            foreach (Feature f in shp.Features)
            {

                //Grasshopper.Kernel.Data.GH_Path path = new Grasshopper.Kernel.Data.GH_Path();
                List<Point3d> pts = new List<Point3d>();


                IList<DotSpatial.Topology.Coordinate> coords = f.Coordinates;

                // Get coords of each point
                foreach (DotSpatial.Topology.Coordinate coord in coords)
                {
                    Point3d pt = new Point3d(coord.X, coord.Y, 0);
                    pts.Add(pt);
                }



                GH_Path p = new GH_Path(pathCount);
                ptTree.AddRange(pts, p);
                pathCount++;
            }


            //Output the data
            DA.SetDataTree(0, keysTree);
            DA.SetDataTree(1, valsTree);
            DA.SetDataTree(2, ptTree);



        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("112907d2-b9fc-45f5-9c06-677b42d95349"); }
        }
    }
}
