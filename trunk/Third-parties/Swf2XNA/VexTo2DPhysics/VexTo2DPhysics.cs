/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Bitmap = System.Drawing.Bitmap;
using DDW.Vex;

namespace DDW.VexTo2DPhysics
{
    public class VexTo2DPhysics
    {
        public List<string> jointKinds = new List<string>(new string[] { 
            "distanceJoint", 
            "revoluteJoint", 
            "prismaticJoint",
            "pullyJoint",
            "gearJoint",
            "mouseJoint"
        });
        public List<string> shapeKinds = new List<string>(new string[] { 
            "circleShape"
        });

        public  List<JointKind> jointKindMap = new List<JointKind>(new JointKind[] { 
            JointKind.Distance, 
            JointKind.Revolute, 
            JointKind.Prismatic, 
            JointKind.Pully, 
            JointKind.Gear, 
            JointKind.Mouse
        });

        public static string OutputDirectory = Directory.GetCurrentDirectory();

        private  Color outlineColor = new Color(0xFF, 0x00, 0x00, 0xFF);
        private  float outlineWidth = 1F;
        private  VexObject curVo;
        private  string curName;
        private  List<string> parsedTypes = new List<string>();

        public Dictionary<string, string> worldData = new Dictionary<string, string>();
        public Dictionary<string, Body2D> bodies = new Dictionary<string, Body2D>();
        public Dictionary<string, Joint2D> joints = new Dictionary<string, Joint2D>();
        public Dictionary<string, Dictionary<string, string>> codeData = new Dictionary<string, Dictionary<string, string>>();

        public Dictionary<uint, string> paths;

        public VexTo2DPhysics()
        {
        }

        public void Convert(VexObject vo, DDW.Swf.SwfCompilationUnit scu)
        {
            curVo = vo;
            if (V2D.OUTPUT_TYPE == OutputType.Xna)
            {
                Gdi.GdiRenderer gr = new Gdi.GdiRenderer();
                Dictionary<string, Bitmap> bmps = gr.GenerateTimelineMappedBitmaps(vo);
                paths = gr.ExportBitmaps(bmps);
            }

            Init(scu);
            ParseActions(scu);
            ParseRootTimeline();
            GenerateActionData();
            GenerateJointData();

            string path = scu.FullPath;
            if (V2D.OUTPUT_TYPE == OutputType.Swf)
            {
                path = path.Replace(".swf", ".as");
                GenActionscript gas = new GenActionscript(this, path);
            }
            else
            {
                path = path.Replace(".swf", ".xml");
                GenXNA gx = new GenXNA(this, path, paths);
            }
        }



        public void Init(DDW.Swf.SwfCompilationUnit scu)
        {
            Body2D ground = new Body2D(V2D.ROOT_NAME, "_root", -1, Rectangle.Empty);
            ground.Transforms = new List<Transform>();
            ground.Transforms.Add(new Transform(0, (uint)(scu.Header.FrameCount * (1 / scu.Header.FrameRate)), new Matrix(1, 0, 0, 1, 0, 0), 1, ColorTransform.Identity));
            bodies.Add(V2D.ROOT_NAME, ground);
        }
        private void ParseActions(DDW.Swf.SwfCompilationUnit scu)
        {
            List<DDW.Swf.DoActionTag> dat = new List<DDW.Swf.DoActionTag>();
            foreach (DDW.Swf.ISwfTag tag in scu.Tags)
            {
                if (tag is DDW.Swf.DoActionTag)
                {
                    dat.Add((DDW.Swf.DoActionTag)tag);
                }
            }
            
            foreach (DDW.Swf.DoActionTag tag in dat)
            {
                DDW.Swf.ConstantPool cp = null;
                List<string> stack = new List<string>();
                for (int i = 0; i < tag.Statements.Count; i++)
                {
                    DDW.Swf.IAction a = tag.Statements[i];
                    if (a is DDW.Swf.ConstantPool)
                    {
                        cp = (DDW.Swf.ConstantPool)a;
                    }
                    else if (a is DDW.Swf.SetMember)
                    {
                        if (stack.Count > 2)
                        {
                            string s0 = stack[0];
                            string s1 = stack[1];
                            string s2 = stack[2];
                            if(!codeData.ContainsKey(s0))
                            {
                                codeData.Add(s0, new Dictionary<string,string>());
                            }
                            Dictionary<string,string> targData = codeData[s0];
                            targData.Add(s1, s2);
                        }
                        stack.Clear();
                    }
                    else if (a is DDW.Swf.Push)
                    {
                        DDW.Swf.Push push = (DDW.Swf.Push)a;
                        for (int j = 0; j < push.Values.Count; j++)
                        {
                            DDW.Swf.IPrimitive v = push.Values[j];
                            switch (v.PrimitiveType)
                            {
                                case DDW.Swf.PrimitiveType.String:
                                    stack.Add(((DDW.Swf.PrimitiveString)v).StringValue);
                                    break;

                                case DDW.Swf.PrimitiveType.Constant8:
                                    stack.Add(cp.Constants[((DDW.Swf.PrimitiveConstant8)v).Constant8Value]);
                                    break;
                                case DDW.Swf.PrimitiveType.Constant16:
                                    stack.Add(cp.Constants[((DDW.Swf.PrimitiveConstant16)v).Constant16Value]);
                                    break;

                                case DDW.Swf.PrimitiveType.Integer:
                                    stack.Add(((DDW.Swf.PrimitiveInteger)v).IntegerValue.ToString());
                                    break;
                                case DDW.Swf.PrimitiveType.Float:
                                    stack.Add(((DDW.Swf.PrimitiveFloat)v).FloatValue.ToString());
                                    break;
                                case DDW.Swf.PrimitiveType.Double:
                                    stack.Add(((DDW.Swf.PrimitiveDouble)v).DoubleValue.ToString());
                                    break;
                                case DDW.Swf.PrimitiveType.Boolean:
                                    stack.Add(((DDW.Swf.PrimitiveBoolean)v).BooleanValue.ToString().ToLowerInvariant());
                                    break;
                                case DDW.Swf.PrimitiveType.Null:
                                    stack.Add("null");
                                    break;
                                case DDW.Swf.PrimitiveType.Undefined:
                                    stack.Add("null");
                                    break;
                            }
                        }
                    }
                }
            }

        }

        private void ParseRootTimeline()
        {
            ParseTimeline(curVo.Root);
        }
        private void ParseTimeline(Timeline t)
        {
            List<IInstance> ris = t.Instances;
            foreach (IInstance instance in ris)
            {
                if (!(instance is Instance))
                {
                    continue;
                }
                Instance inst = (Instance)instance;
                IDefinition def = curVo.Definitions[inst.DefinitionId];
                if (inst.Name == null)
                {
                    inst.Name = "inst" + inst.Depth;
                }
                curName = def.Name;

                if (jointKinds.Contains(curName))
                {
                    // joint object
                    JointKind jointKind = jointKindMap[jointKinds.IndexOf(curName)];
                    ParseJoint(jointKind, inst);
                }
                else if (def is Timeline)
                {
                    // body object
                    Timeline tl = (Timeline)def;
                    Body2D b2d = new Body2D(inst.Name, def.Name, (int)inst.Depth, def.StrokeBounds);
                    bodies.Add(inst.Name, b2d);

                    b2d.Transforms = inst.Transformations;
                    if (HasShape(curVo, tl))
                    {
                        foreach (IInstance tlInst in tl.Instances)
                        {
                            IDefinition def2 = curVo.Definitions[tlInst.DefinitionId];

                            string nm2 = def2.Name;
                            bool isShapeDef = IsShapeDef(def2);
                            if (isShapeDef)
                            {
                                b2d.AddShapes(curVo, def2, (Instance)tlInst);
                            }
                            else
                            {
                                ParseBodyImage(b2d, def2, inst);
                            }
                        }
                    }
                    else
                    {
                        // bkg, not box2D element
                        ParseBodyImage(b2d, def, inst);
                    }
                    parsedTypes.Add(curName);
                }
            }
        }
        private bool HasShape(VexObject vo, Timeline tl)
        {
            bool result = false;
            foreach (IInstance inst in tl.Instances)
            {
                IDefinition def = vo.Definitions[inst.DefinitionId];
                if (IsShapeDef(def))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        private void ParseJoint(JointKind jointKind, Instance inst)
        {
            string nm = inst.Name;
            int firstUs = nm.IndexOf('_');
            string jName = (firstUs > -1) ? nm.Substring(0, firstUs) : nm;
            string targName = (firstUs > -1) ? nm.Substring(firstUs + 1) : "";

            Matrix m = inst.Transformations[0].Matrix;
            float rot = (float)((m.GetMatrixComponents().Rotation + 90) * Math.PI / 180);

            Point p = new Point(
                inst.Transformations[0].Matrix.TranslateX,
                inst.Transformations[0].Matrix.TranslateY);

            if(joints.ContainsKey(jName))
            {
                joints[jName].AppendData(p, rot);
            }
            else
            {
                joints.Add(jName, new Joint2D(jName, targName, jointKind, p, rot));
            }
            if (inst.Transformations.Count > 1) // pulley defined by joints on frame 2
            {
                joints[jName].Transforms.Add(new Point(
                    inst.Transformations[1].Matrix.TranslateX,
                    inst.Transformations[1].Matrix.TranslateY));
            }
        }


        private  void ParseBodyImage(Body2D b2d, IDefinition sy, Instance inst)
        {
            if (V2D.OUTPUT_TYPE == OutputType.Swf)
            {
                string path = VexTo2DPhysics.OutputDirectory + "/" + curVo.Name + ".swf";
                path = path.Replace('\\', '/');
                b2d.SetSymbol(sy.Name, path);
            }
            else if (V2D.OUTPUT_TYPE == OutputType.Xna)
            {
                string bmpPath = "";
                if (sy is Timeline)
                {
                    Timeline tl = (Timeline)sy;
                    bmpPath = paths[tl.Id];
                }
                else if (sy is Symbol)
                {
                    bmpPath = paths[sy.Id];
                }
                string path = VexTo2DPhysics.OutputDirectory + "/" + bmpPath;
                path = path.Replace('\\', '/');
                b2d.SetSymbol(sy.Name, path);
            }
        }

        private  bool IsShapeDef(IDefinition def)
        {
            bool result = true;
            if (def.Name != null && shapeKinds.Contains(def.Name))
            {
                // is shape
            }
            else if (def is Symbol)
            {
                List<Shape> shapes = ((Symbol)def).Shapes;
                foreach (Shape sh in shapes)
                {
                    if (sh.Fill != null || !(sh.Stroke is SolidStroke))
                    {
                        result = false;
                        break;
                    }
                    SolidStroke sf = (SolidStroke)sh.Stroke;
                    if ((sf.Color != outlineColor) || (sf.LineWidth > outlineWidth))
                    {
                        result = false;
                        break;
                    }
                }
            }
            else if (def is Timeline)
            {
                for (int i = 0; i < ((Timeline)def).Instances.Count; i++)
                {
                    IInstance inst = ((Timeline)def).Instances[i];
                    IDefinition def2 = curVo.Definitions[inst.DefinitionId];
                    result = IsShapeDef(def2);
                    if (!result)
                    {
                        break;
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }
        private void GenerateActionData()
        {
            foreach (string k in codeData.Keys)
	        {
                if (k == "world")
                {
                    foreach (string s in codeData[k].Keys)
                    {
                        if(worldData.ContainsKey(s))
                        {
                            worldData[s] = codeData[k][s];
                        }
                        else
                        {
                            worldData.Add(s, codeData[k][s]);
                        }
                    }
                    //worldData.Add(k, codeData[);
                }
        		else if(joints.ContainsKey(k))
                {
                    Dictionary<string, string> d = codeData[k];
                    foreach (string s in d.Keys)
                    {
                        joints[k].data.Add(s, d[s]);
                    }
                }
            }
            if (!worldData.ContainsKey("Width"))
            {
                worldData["Width"] = curVo.ViewPort.Size.Width.ToString();
            }
            if (!worldData.ContainsKey("Height"))
            {
                worldData["Height"] = curVo.ViewPort.Size.Height.ToString();
            }
                     
        }
        private List<Body2D> FindTargetUnderPoint(Point p)
        {
            List<Body2D> result = new List<Body2D>();
            foreach (Body2D b in bodies.Values)
            {
                if (b.IsPointInside(p))
                {
                    result.Add(b);
                }
            }
            result.Sort();
            return result;
        }
        private void EnsureBodyTags(Joint2D j)
        {
            bool hasB1 = j.data.ContainsKey("body1");
            bool hasB2 = j.data.ContainsKey("body2");
            bool b1FromCode = hasB1;

            if (!hasB1 || !hasB2)
            {
                List<Body2D> hits = FindTargetUnderPoint(j.Locations[0]);
                if(!hasB1 && hits.Count > 0)
                {
                    j.data.Add("body1", hits[0].InstanceName);
                    hasB1 = true;
                }

                if (!hasB2)
                {
                    Body2D b2 = null;
                    if (j.Locations.Count > 1)// && j.JointKind == JointKind.Distance)
                    {
                        List<Body2D> b2L  = FindTargetUnderPoint(j.Locations[1]);
                        
                        if (b2L.Count > 0)
                        {
                            b2 = b2L[0];
                        }
                        else
                        {
                            // ground
                        }
                    }
                    else if (hits.Count > 1)
                    {
                        b2 = hits[1];
                    }
                    else if (b1FromCode && hits.Count > 0)
                    {
                        b2 = hits[0];
                    }

                    if (!hasB1)
                    {
                        j.data.Add("body1", V2D.ROOT_NAME);
                    }

                    if (b2 != null)
                    {
                        j.data.Add("body2", b2.InstanceName);
                        hasB2 = true;
                    }
                    else
                    {
                        j.data.Add("body2", V2D.ROOT_NAME);
                    }
                }
            }

            // flip if a ground ref (I think all joints to ground use ground on body 1..?)
            if (j.data["body2"] == V2D.ROOT_NAME)
            {
                string temp = j.data["body2"];
                j.data["body2"] = j.data["body1"];
                j.data["body1"] = temp;
                if (j.Locations.Count > 1)
                {
                    Point temp2 = j.Locations[0];
                    j.Locations[0] = j.Locations[1];
                    j.Locations[1] = temp2;
                }
            }
        }
        private  void GenerateJointData()
        {
            foreach (Joint2D j in joints.Values)
            {
                string jName = j.Name;
                EnsureBodyTags(j);
                string b1Name = j.data["body1"];
                string b2Name = j.data["body2"];
                Body2D b1Body = bodies[b1Name];
                Body2D b2Body = bodies[b2Name];

                JointKind jointKind = j.JointKind;

                if (!j.data.ContainsKey("location"))
                {
                    if (V2D.OUTPUT_TYPE == OutputType.Swf)
                    {
                        j.data.Add("location", "#" + j.Locations[0].ToString());
                    }
                    else
                    {
                        j.data.Add("X", "#" + j.Locations[0].X.ToString());
                        j.data.Add("Y", "#" + j.Locations[0].Y.ToString());
                    }
                }
                if ((j.Locations.Count > 1) && (jointKind == JointKind.Distance || jointKind == JointKind.Pully))
                {
                    if (!j.data.ContainsKey("location2"))
                    {
                        if (V2D.OUTPUT_TYPE == OutputType.Swf)
                        {
                            j.data.Add("location2", "#" + j.Locations[1].ToString());
                        }
                        else
                        {
                            j.data.Add("X2", "#" + j.Locations[1].X.ToString());
                            j.data.Add("Y2", "#" + j.Locations[1].Y.ToString());
                        }
                    }
                    if (jointKind == JointKind.Pully)
                    {
                        Point groundAnchor1;
                        Point groundAnchor2;
                        if (b1Body.Transforms.Count > 1)
                        {
                            groundAnchor1 = new Point(
                                b1Body.Transforms[1].Matrix.TranslateX,
                                b1Body.Transforms[1].Matrix.TranslateY);
                            groundAnchor2 = new Point(
                                b2Body.Transforms[1].Matrix.TranslateX,
                                b2Body.Transforms[1].Matrix.TranslateY);
                        }
                        else
                        {
                            groundAnchor1 = j.Transforms[0];
                            groundAnchor2 = j.Transforms[1];
                        }

                        float d1x = groundAnchor1.X - j.Locations[0].X;// m1a.TranslateX;
                        float d1y = groundAnchor1.Y - j.Locations[0].Y;//m1a.TranslateY;
                        float d2x = groundAnchor2.X - j.Locations[1].X;//m2a.TranslateX;
                        float d2y = groundAnchor2.Y - j.Locations[1].Y;//m2a.TranslateY;
                        float maxLength1 = (float)Math.Sqrt(d1x * d1x + d1y * d1y);
                        float maxLength2 = (float)Math.Sqrt(d2x * d2x + d2y * d2y);
                        //j.data.Add("groundAnchor1", "#" + groundAnchor1.ToString());
                        //j.data.Add("groundAnchor2", "#" + groundAnchor2.ToString());
                        j.data.Add("groundAnchor1X", "#" + groundAnchor1.X.ToString());
                        j.data.Add("groundAnchor1Y", "#" + groundAnchor1.Y.ToString());
                        j.data.Add("groundAnchor2X", "#" + groundAnchor2.X.ToString());
                        j.data.Add("groundAnchor2Y", "#" + groundAnchor2.Y.ToString());
                        j.data.Add("maxLength1", "#" + maxLength1.ToString());
                        j.data.Add("maxLength2", "#" + maxLength2.ToString());
                    }
                }
                else if (b1Body.Transforms.Count > 1 || b2Body.Transforms.Count > 1)
                {
                    List<Transform> tr = (b1Body.Transforms.Count > 1) ? 
                        b1Body.Transforms : b2Body.Transforms;
                    if (jointKind == JointKind.Revolute)
                    {
                        float start = (float)(tr[0].Matrix.GetMatrixComponents().Rotation / 180 * Math.PI);
                        float rmin = (float)(tr[1].Matrix.GetMatrixComponents().Rotation / 180 * Math.PI);
                        float rmax = (float)(tr[2].Matrix.GetMatrixComponents().Rotation / 180 * Math.PI);
                        rmin = rmin - start;
                        rmax = rmax - start;
                        if (rmin > 0)
                        {
                            rmin = (float)(Math.PI * -2 + rmin);
                        }
                        j.data.Add("min", "#" + rmin.ToString());
                        j.data.Add("max", "#" + rmax.ToString());

                    }
                    else if (jointKind == JointKind.Prismatic)
                    {
                        Point a = new Point(
                            tr[0].Matrix.TranslateX,
                            tr[0].Matrix.TranslateY);
                        Point p0 = new Point(
                            tr[1].Matrix.TranslateX,
                            tr[1].Matrix.TranslateY);
                        Point p1 = new Point(
                            tr[2].Matrix.TranslateX,
                            tr[2].Matrix.TranslateY);

                        double len = Math.Sqrt((p1.Y - p0.Y) * (p1.Y - p0.Y) + (p1.X - p0.X) * (p1.X - p0.X));
                        double r = ((p0.Y - a.Y) * (p0.Y - p1.Y) - (p0.X - a.X) * (p0.X - p1.X)) / (len * len);

                        float axisX = p1.X - p0.X;
                        float axisY = p1.Y - p0.Y;
                        float maxAxis = Math.Abs(axisX) > Math.Abs(axisY) ? axisX : axisY;
                        axisX /= maxAxis;
                        axisY /= maxAxis;

                        Point ap0 = new Point(p0.X - a.X, p0.Y - a.Y);
                        Point ap1 = new Point(p1.X - a.X, p1.Y - a.Y);
                        float min = (float)-(Math.Abs(r * len));
                        float max = (float)((1 - Math.Abs(r)) * len);
                        // Point ab = new Point(b.X - a.X, b.Y - a.Y);

                        // float r0 = (ap0.X * ab.X + ap0.Y * ab.Y) / (w * w); // dot product
                        //float r1 = (ap1.X * ab.X + ap1.Y * ab.Y) / (w * w); // dot product

                        j.data.Add("axisX", "#" + axisX.ToString());
                        j.data.Add("axisY", "#" + axisY.ToString());
                        j.data.Add("min", "#" + min.ToString());
                        j.data.Add("max", "#" + max.ToString());
                    }
                }
            }
        }

    }
}
