using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using MissionPlanner.Mavlink;

namespace MissionPlanner.Log
{
    public class LogMap
    {
        public static void MapLogs(string[] logs)
        {
            foreach (var logfile in logs)
            {
                if (File.Exists(logfile + ".jpg"))
                    continue;

                double minx = 99999;
                double maxx = -99999;
                double miny = 99999;
                double maxy = -99999;

                Dictionary<int,List<PointLatLngAlt>> loc_list = new Dictionary<int, List<PointLatLngAlt>>();

                try
                {
                    if (logfile.ToLower().EndsWith(".tlog"))
                    {
                        using (MAVLinkInterface mine = new MAVLinkInterface())
                        using (
                            mine.logplaybackfile =
                                new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            )
                        {
                            mine.logreadmode = true;

                            while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                            {
                                MAVLink.MAVLinkMessage packet = mine.readPacket();

                                if (packet.Length < 5)
                                    continue;

                                try
                                {
                                    if (MainV2.speechEngine != null)
                                        MainV2.speechEngine.SpeakAsyncCancelAll();
                                }
                                catch
                                {
                                }

                                if (packet.msgid == (byte) MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT)
                                {
                                    var loc = packet.ToStructure<MAVLink.mavlink_global_position_int_t>();

                                    if (loc.lat == 0 || loc.lon == 0)
                                        continue;

                                    var id = MAVList.GetID(packet.sysid, packet.compid);

                                    if (!loc_list.ContainsKey(id))
                                        loc_list[id] = new List<PointLatLngAlt>();

                                    loc_list[id].Add(new PointLatLngAlt(loc.lat/10000000.0f, loc.lon/10000000.0f));

                                    minx = Math.Min(minx, loc.lon/10000000.0f);
                                    maxx = Math.Max(maxx, loc.lon/10000000.0f);
                                    miny = Math.Min(miny, loc.lat/10000000.0f);
                                    maxy = Math.Max(maxy, loc.lat/10000000.0f);
                                }
                            }
                        }
                    }
                    else if (logfile.ToLower().EndsWith(".bin") || logfile.ToLower().EndsWith(".log"))
                    {
                        bool bin = logfile.ToLower().EndsWith(".bin");

                        BinaryLog binlog = new BinaryLog();
                        DFLog dflog = new DFLog();

                        using (var st = File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (StreamReader sr = new StreamReader(st))
                            {
                                loc_list[0] = new List<PointLatLngAlt>();

                                while (sr.BaseStream.Position < sr.BaseStream.Length)
                                {
                                    string line = "";

                                    if (bin)
                                    {
                                        line = binlog.ReadMessage(sr.BaseStream);
                                    }
                                    else
                                    {
                                        line = sr.ReadLine();
                                    }

                                    if (line.StartsWith("FMT"))
                                    {
                                        dflog.FMTLine(line);
                                    }
                                    else if (line.StartsWith("GPS"))
                                    {
                                        var item = dflog.GetDFItemFromLine(line, 0);

                                        var lat = double.Parse(item.items[dflog.FindMessageOffset(item.msgtype, "Lat")]);
                                        var lon = double.Parse(item.items[dflog.FindMessageOffset(item.msgtype, "Lng")]);

                                        if (lat == 0 || lon == 0)
                                            continue;                                            

                                        loc_list[0].Add(new PointLatLngAlt(lat, lon));

                                        minx = Math.Min(minx, lon);
                                        maxx = Math.Max(maxx, lon);
                                        miny = Math.Min(miny, lat);
                                        maxy = Math.Max(maxy, lat);
                                    }
                                }
                            }
                        }
                    }


                    if (loc_list.Count > 0 && loc_list.First().Value.Count > 10)
                    {
                        // add a bit of buffer
                        var area = RectLatLng.FromLTRB(minx - 0.001, maxy + 0.001, maxx + 0.001, miny - 0.001);
                        var map = GetMap(area);

                        var grap = Graphics.FromImage(map);

                        Color[] colours =
                        {
                            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo,
                            Color.Violet, Color.Pink
                        };

                        int a = 0;
                        foreach (var locs in loc_list.Values)
                        {
                            PointF lastpoint = new PointF();
                            var pen = new Pen(colours[a%(colours.Length - 1)]);

                            foreach (var loc in locs)
                            {
                                PointF newpoint = GetPixel(area, loc, map.Size);

                                if (!lastpoint.IsEmpty)
                                    grap.DrawLine(pen, lastpoint, newpoint);

                                lastpoint = newpoint;
                            }

                            a++;
                        }

                        map.Save(logfile + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                        map.Dispose();

                        map = null;
                    }
                    else
                    {
                        DoTextMap(logfile + ".jpg", "No gps data");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("Mavlink 0.9"))
                        DoTextMap(logfile + ".jpg", "Old log\nMavlink 0.9");

                    continue;
                }
            }
        }

        static void DoTextMap(string jpgname, string text)
        {
            var map = new Bitmap(100, 100);

            var grap = Graphics.FromImage(map);

            grap.DrawString(text, SystemFonts.DefaultFont, Brushes.Red, 0, 0, StringFormat.GenericDefault);

            map.Save(jpgname, System.Drawing.Imaging.ImageFormat.Jpeg);

            map.Dispose();

            map = null;
        }

        static PointF GetPixel(RectLatLng area, PointLatLngAlt loc, Size size)
        {
            double lon = loc.Lng;
            double lat = loc.Lat;

            double lonscale = (lon - area.Left)*(size.Width - 0)/(area.Right - area.Left) + 0;

            double latscale = (lat - area.Top)*(size.Height - 0)/(area.Bottom - area.Top) + 0;

            return new PointF((float) lonscale, (float) latscale);
        }

        static Bitmap GetMap(RectLatLng area)
        {
            GMapProvider type = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            PureProjection prj = type.Projection;

            int zoom = 16;

            GPoint topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, zoom);
            GPoint rightButtomPx = prj.FromLatLngToPixel(area.Bottom, area.Right, zoom);
            GPoint pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);

            // zoom based on pixel density
            while (pxDelta.X > 2000)
            {
                zoom--;

                // current area
                topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, zoom);
                rightButtomPx = prj.FromLatLngToPixel(area.Bottom, area.Right, zoom);
                pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);
            }

            // get type list at new zoom level
            List<GPoint> tileArea = prj.GetAreaTileList(area, zoom, 0);

            int padding = 10;

            Bitmap bmpDestination = new Bitmap((int) pxDelta.X + padding*2, (int) pxDelta.Y + padding*2);

            {
                using (Graphics gfx = Graphics.FromImage(bmpDestination))
                {
                    gfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                    // get tiles & combine into one
                    foreach (var p in tileArea)
                    {
                        Console.WriteLine("Downloading[" + p + "]: " + tileArea.IndexOf(p) + " of " + tileArea.Count);

                        foreach (var tp in type.Overlays)
                        {
                            Exception ex;
                            using (GMapImage tile = GMaps.Instance.GetImageFrom(tp, p, zoom, out ex) as GMapImage)
                            {
                                if (tile != null)
                                {
                                    using (tile)
                                    {
                                        long x = p.X*prj.TileSize.Width - topLeftPx.X + padding;
                                        long y = p.Y*prj.TileSize.Width - topLeftPx.Y + padding;
                                        {
                                            gfx.DrawImage(tile.Img, x, y, prj.TileSize.Width, prj.TileSize.Height);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return bmpDestination;
            }
        }
    }
}