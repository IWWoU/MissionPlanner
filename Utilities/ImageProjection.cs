using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.HIL;

namespace MissionPlanner.Utilities
{
    public class ImageProjection
    {
        public const double rad2deg = (180 / System.Math.PI);
        public const double deg2rad = (1.0 / rad2deg);

        static double fovCalc(double fov, double distance)
        {
            return distance * Math.Tan(fov * deg2rad);
        }

        //http://www.chrobotics.com/library/understanding-euler-angles

        // x = north / roll
        // y = pitch / east
        // z = yaw / down

        public static List<PointLatLngAlt> calc(PointLatLngAlt plla, double R, double P, double Y, double hfov, double vfov)
        {
            // alt should be asl
            // P pitch where the center os pointing ie -80
            // R roll 

            // if roll and pitch is 0, use the quick method.
            if (R == 0 && P == 0)
            {
                // calc fov in m on the ground at 0 alt
                var fovh = Math.Tan(hfov / 2.0 * deg2rad) * plla.Alt;
                var fovv = Math.Tan(vfov / 2.0 * deg2rad) * plla.Alt;
                var fovd = Math.Sqrt(fovh * fovh + fovv * fovv);

                // where we put our footprint
                var ans2 = new List<PointLatLngAlt>();

                // calc bearing from center to corner
                var bearing1 = Math.Atan2(fovh, fovv) * rad2deg;

                // calc first corner point
                var newpos1 = plla.newpos(bearing1 + Y, Math.Sqrt(fovh * fovh + fovv * fovv));
                // set alt to 0, as we used the hypot for distance and fov is based on 0 alt
                newpos1.Alt = 0;
                // calc intersection from center to new point and return ground hit point
                var cen1 = calcIntersection(plla, newpos1);
                // add to our footprint poly
                ans2.Add(cen1);
                addtomap(cen1, "cen1");

                //repeat

                newpos1 = plla.newpos(Y - bearing1, Math.Sqrt(fovh * fovh + fovv * fovv));
                newpos1.Alt = 0;
                cen1 = calcIntersection(plla, newpos1);
                ans2.Add(cen1);
                addtomap(cen1, "cen2");

                newpos1 = plla.newpos(bearing1 + Y - 180, Math.Sqrt(fovh * fovh + fovv * fovv));
                newpos1.Alt = 0;
                cen1 = calcIntersection(plla, newpos1);
                ans2.Add(cen1);
                addtomap(cen1, "cen3");

                newpos1 = plla.newpos(Y - bearing1 - 180, Math.Sqrt(fovh * fovh + fovv * fovv));
                newpos1.Alt = 0;
                cen1 = calcIntersection(plla, newpos1);
                ans2.Add(cen1);
                addtomap(cen1, "cen4");


                addtomap(plla, "plane");

                return ans2;
            }

            
            GMapPolygon poly = new GMapPolygon(new List<PointLatLng>(), "rect");

            double frontangle = (P*0) + vfov/2;
            double backangle = (P*0) - vfov/2;
            double leftangle = (R*0) + hfov/2;
            double rightangle = (R*0) - hfov/2;


            addtomap(plla, "plane");

            Matrix3 dcm = new Matrix3();

            dcm.from_euler(R * deg2rad, P * deg2rad, Y * deg2rad);
            dcm.normalize();

            Vector3 center1 = new Vector3(0, 0, 10000);

            var test = dcm * center1;

            var bearing2 = Math.Atan2(test.y, test.x) * rad2deg;

            var newpos2 = plla.newpos(bearing2, Math.Sqrt(test.x * test.x + test.y * test.y));

            newpos2.Alt -= test.z;

            var cen = calcIntersection(plla, newpos2);

            var dist = plla.GetDistance(cen);

            addtomap(cen, "center "+ dist.ToString("0"));

            //
            dcm.from_euler(R * deg2rad, P * deg2rad, Y * deg2rad);
            dcm.rotate(new Vector3(rightangle * deg2rad, 0, 0));
            dcm.normalize();
            dcm.rotate(new Vector3(0, frontangle * deg2rad, 0));
            dcm.normalize();
            /*
            var mx = new Matrix3();
            var my = new Matrix3();
            var mz = new Matrix3();

            mx.from_euler((rightangle + R) * deg2rad, 0, 0);
            my.from_euler(0, (frontangle + P) * deg2rad, 0);
            mz.from_euler(0, 0, Y * deg2rad);

            printdcm(mx);
            printdcm(my);
            printdcm(mz);
            printdcm(my * mx);
            printdcm(mz * my * mx);

            test = mz * my * mx * center1;
            */
            test = dcm * center1;  
              
            bearing2 = (Math.Atan2(test.y, test.x) * rad2deg);

            newpos2 = plla.newpos(bearing2, Math.Sqrt(test.x * test.x + test.y * test.y));

            newpos2.Alt -= test.z;

            //addtomap(newpos2, "tr2");

            var groundpointtr = calcIntersection(plla, newpos2);

            poly.Points.Add(groundpointtr);

            addtomap(groundpointtr, "tr");

            //
            dcm.from_euler(R * deg2rad, P * deg2rad, Y * deg2rad);
            dcm.rotate(new Vector3(leftangle * deg2rad, 0, 0));
            dcm.normalize();
            dcm.rotate(new Vector3(0, frontangle * deg2rad, 0));
            dcm.normalize();

            test = dcm * center1;

            bearing2 = Math.Atan2(test.y, test.x)*rad2deg;

            newpos2 = plla.newpos(bearing2, Math.Sqrt(test.x * test.x + test.y * test.y));

            newpos2.Alt -= test.z;

            var groundpointtl = calcIntersection(plla, newpos2);

            poly.Points.Add(groundpointtl);

            addtomap(groundpointtl, "tl");

            //
            dcm.from_euler(R * deg2rad, P * deg2rad, Y * deg2rad);
            dcm.rotate(new Vector3(leftangle * deg2rad, 0, 0));
            dcm.normalize();
            dcm.rotate(new Vector3(0, backangle * deg2rad, 0));
            dcm.normalize();

            test = dcm * center1;

            bearing2 = Math.Atan2(test.y, test.x) * rad2deg;

            newpos2 = plla.newpos(bearing2, Math.Sqrt(test.x * test.x + test.y * test.y));

            newpos2.Alt -= test.z;

            var groundpointbl = calcIntersection(plla, newpos2);

            poly.Points.Add(groundpointbl);

            addtomap(groundpointbl, "bl");

            //
            dcm.from_euler(R * deg2rad, P * deg2rad, Y * deg2rad);
            dcm.rotate(new Vector3(rightangle * deg2rad, 0, 0));
            dcm.normalize();
            dcm.rotate(new Vector3(0, backangle * deg2rad, 0));
            dcm.normalize(); 

            test = dcm * center1;

            bearing2 = Math.Atan2(test.y, test.x) * rad2deg;

            newpos2 = plla.newpos(bearing2, Math.Sqrt(test.x * test.x + test.y * test.y));

            newpos2.Alt -= test.z;

            var groundpointbr = calcIntersection(plla, newpos2);

            poly.Points.Add(groundpointbr);

            addtomap(groundpointbr, "br");

            //

            polygons.Polygons.Clear();
            polygons.Polygons.Add(poly);

            var ans = new List<PointLatLngAlt>();
            ans.Add(groundpointtl);
            ans.Add(groundpointtr);
            ans.Add(groundpointbr);
            ans.Add(groundpointbl);

            return ans;
        }

        private static void printdcm(Matrix3 dcm)
        {
            double R = 0;
            double P = 0;
            double Y = 0;

            dcm.to_euler(ref R, ref P, ref Y);

            Console.WriteLine("{0} {1} {2}", R * rad2deg, P * rad2deg, Y * rad2deg);
        }

        // polar to rectangular
        static void newpos(ref double x, ref double y, double bearing, double distance)
        {
            double degN = 90 - bearing;
            if (degN < 0)
                degN += 360;
            x = x + distance * Math.Cos(degN * deg2rad);
            y = y + distance * Math.Sin(degN * deg2rad);
        }

        // polar to rectangular
        static utmpos newpos(utmpos input, double bearing, double distance)
        {
            double degN = 90 - bearing;
            if (degN < 0)
                degN += 360;
            double x = input.x + distance * Math.Cos(degN * deg2rad);
            double y = input.y + distance * Math.Sin(degN * deg2rad);

            return new utmpos(x, y, input.zone);
        }

        static PointLatLngAlt calcIntersection(PointLatLngAlt plla, PointLatLngAlt dest, int step = 100)
        {
            int distout = 10;
            PointLatLngAlt newpos = PointLatLngAlt.Zero;

            var dist = plla.GetDistance(dest);
            var Y = plla.GetBearing(dest);

            // 20 km
            while (distout < (dist+100))
            {
                // get a projected point to test intersection against - not using slope distance
                PointLatLngAlt newposdist = plla.newpos(Y, distout);
                newposdist.Alt = srtm.getAltitude(newposdist.Lat, newposdist.Lng).alt;

                // get another point 'step' infront
                PointLatLngAlt newposdist2 = plla.newpos(Y, distout + step);
                newposdist2.Alt = srtm.getAltitude(newposdist2.Lat, newposdist2.Lng).alt;

                // x is dist from plane, y is alt
                var newpoint = FindLineIntersection(new PointF(0, (float)plla.Alt),
                    new PointF((float)dist, (float)dest.Alt),
                    new PointF((float)distout, (float)newposdist.Alt),
                    new PointF((float)distout + step, (float)newposdist2.Alt));

                if (newpoint.X != 0)
                {
                    newpos = plla.newpos(Y, newpoint.X);
                    newpos.Alt = newpoint.Y;

                    return newpos;
                }

                distout += step;
            }

            //addtomap(newpos, distout.ToString());

            return plla;
        }

        static GMapOverlay polygons = new GMapOverlay("polygons");
        static GMapControl map = null;

        static void DoDebug()
        {
            if (map != null)
            {
                if (map.Overlays.Count != 0)
                {
                    while (polygons.Markers.Count > 7)
                        polygons.Markers.RemoveAt(0);
                    //polygons.Markers.Clear();
                    return;
                }
            }

            polygons = new GMapOverlay("polygons");
            map = new GMapControl();
            var form = new Form() { Size = new Size(1024, 768), WindowState = FormWindowState.Maximized };
            map.Dock = DockStyle.Fill;
            map.MapProvider = GMapProviders.GoogleSatelliteMap;
            map.MaxZoom = 20;
            map.Overlays.Add(polygons);
            form.Controls.Add(map);
            form.Show();
        }

        static void addtomap(PointLatLngAlt pos, string tag)
        {
            if (map != null)
            {
                polygons.Markers.Add(new GMarkerGoogle(pos, GMarkerGoogleType.blue)
                {
                    ToolTipText = tag,
                    ToolTipMode = MarkerTooltipMode.Always
                });

                map.ZoomAndCenterMarkers("polygons");
            }
        }

        public static PointF FindLineIntersection(PointF start1, PointF end1, PointF start2, PointF end2)
        {
            double denom = ((end1.X - start1.X) * (end2.Y - start2.Y)) - ((end1.Y - start1.Y) * (end2.X - start2.X));
            //  AB & CD are parallel         
            if (denom == 0)
                return new PointF();
            double numer = ((start1.Y - start2.Y) * (end2.X - start2.X)) - ((start1.X - start2.X) * (end2.Y - start2.Y));
            double r = numer / denom;
            double numer2 = ((start1.Y - start2.Y) * (end1.X - start1.X)) - ((start1.X - start2.X) * (end1.Y - start1.Y));
            double s = numer2 / denom;
            if ((r < 0 || r > 1) || (s < 0 || s > 1))
                return new PointF();
            // Find intersection point      
            PointF result = new PointF();
            result.X = (float)(start1.X + (r * (end1.X - start1.X)));
            result.Y = (float)(start1.Y + (r * (end1.Y - start1.Y)));
            return result;
        }
    }
}
