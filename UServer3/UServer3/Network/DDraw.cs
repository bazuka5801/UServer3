using System.Globalization;
using RakNet.Network;
using SapphireEngine;
using UnityEngine;
using UServer3.Rust;

namespace UServer3.Network
{
    public class DDraw
    {
        internal static string Line(Vector3 from, Vector3 to, Color color, float duration)
        {
            return SendClientCommand("ddraw.line", duration.ToString().Replace(',', '.'), color, ToCSharpString(from), ToCSharpString(to));
        }

        internal static string Arrow(Vector3 from, Vector3 to, float headSize, Color color, float duration)
        {
            return SendClientCommand("ddraw.arrow", duration.ToString().Replace(',', '.'), color, ToCSharpString(from), ToCSharpString(to), headSize);
        }

        internal static string Sphere(Vector3 pos, float radius, Color color, float duration)
        {
            return SendClientCommand("ddraw.sphere", duration.ToString().Replace(',', '.'), color, ToCSharpString(pos), radius);
        }

        internal static string Text(Vector3 pos, string text, Color color, float duration)
        {
            return SendClientCommand("ddraw.text", duration.ToString().Replace(',', '.'), color, ToCSharpString(pos), text);
        }

        internal static string Box(Vector3 pos, float size, Color color, float duration)
        {
            return SendClientCommand("ddraw.box", duration.ToString().Replace(',', '.'), color, ToCSharpString(pos), size);
        }

        public static void DrawBox( Vector3 center, Quaternion rotation, Vector3 size, Color color, float duration)
        {
            size /= 2;
            var point1 = RotatePointAroundPivot( new Vector3( center.x + size.x, center.y + size.y, center.z + size.z ), center, rotation );
            var point2 = RotatePointAroundPivot( new Vector3( center.x + size.x, center.y - size.y, center.z + size.z ), center, rotation );
            var point3 = RotatePointAroundPivot( new Vector3( center.x + size.x, center.y + size.y, center.z - size.z ), center, rotation );
            var point4 = RotatePointAroundPivot( new Vector3( center.x + size.x, center.y - size.y, center.z - size.z ), center, rotation );
            var point5 = RotatePointAroundPivot( new Vector3( center.x - size.x, center.y + size.y, center.z + size.z ), center, rotation );
            var point6 = RotatePointAroundPivot( new Vector3( center.x - size.x, center.y - size.y, center.z + size.z ), center, rotation );
            var point7 = RotatePointAroundPivot( new Vector3( center.x - size.x, center.y + size.y, center.z - size.z ), center, rotation );
            var point8 = RotatePointAroundPivot( new Vector3( center.x - size.x, center.y - size.y, center.z - size.z ), center, rotation );

            SendClientCommand( "ddraw.line", duration, color, point1, point2 );
            SendClientCommand( "ddraw.line", duration, color, point1, point3 );
            SendClientCommand( "ddraw.line", duration, color, point1, point5 );
            SendClientCommand( "ddraw.line", duration, color, point4, point2 );
            SendClientCommand( "ddraw.line", duration, color, point4, point3 );
            SendClientCommand( "ddraw.line", duration, color, point4, point8 );

            SendClientCommand( "ddraw.line", duration, color, point5, point6 );
            SendClientCommand( "ddraw.line", duration, color, point5, point7 );
            SendClientCommand( "ddraw.line", duration, color, point6, point2 );
            SendClientCommand( "ddraw.line", duration, color, point8, point6 );
            SendClientCommand( "ddraw.line", duration, color, point7, point3 );
            SendClientCommand( "ddraw.line", duration, color, point8, point7 );
        }

        
        public static void DrawBox(Vector3 center, Vector3 size, Color color, float duration)
        {
            size /= 2;
            var point1 =  new Vector3( center.x + size.x, center.y + size.y, center.z + size.z );
            var point2 =  new Vector3( center.x + size.x, center.y - size.y, center.z + size.z );
            var point3 =  new Vector3( center.x + size.x, center.y + size.y, center.z - size.z );
            var point4 =  new Vector3( center.x + size.x, center.y - size.y, center.z - size.z );
            var point5 =  new Vector3( center.x - size.x, center.y + size.y, center.z + size.z );
            var point6 =  new Vector3( center.x - size.x, center.y - size.y, center.z + size.z );
            var point7 =  new Vector3( center.x - size.x, center.y + size.y, center.z - size.z );
            var point8 =  new Vector3( center.x - size.x, center.y - size.y, center.z - size.z );

            SendClientCommand( "ddraw.line", duration, color, point1, point2 );
            SendClientCommand( "ddraw.line", duration, color, point1, point3 );
            SendClientCommand( "ddraw.line", duration, color, point1, point5 );
            SendClientCommand( "ddraw.line", duration, color, point4, point2 );
            SendClientCommand( "ddraw.line", duration, color, point4, point3 );
            SendClientCommand( "ddraw.line", duration, color, point4, point8 );

            SendClientCommand( "ddraw.line", duration, color, point5, point6 );
            SendClientCommand( "ddraw.line", duration, color, point5, point7 );
            SendClientCommand( "ddraw.line", duration, color, point6, point2 );
            SendClientCommand( "ddraw.line", duration, color, point8, point6 );
            SendClientCommand( "ddraw.line", duration, color, point7, point3 );
            SendClientCommand( "ddraw.line", duration, color, point8, point7 );
        }
        
        static Vector3 RotatePointAroundPivot( Vector3 point, Vector3 pivot, Quaternion rotation )
        {
            return rotation * ( point - pivot ) + pivot;
        }

        private static string SendClientCommand(string strCommand, params object[] args)
        {
            string builded_command = BuildCommand(strCommand, args);
            if (VirtualServer.BaseServer.IsConnected())
            {
                BasePlayer.LocalPlayer.SetAdminStatus(true);
                VirtualServer.BaseServer.write.Start();
                VirtualServer.BaseServer.write.PacketID(Message.Type.ConsoleCommand);
                VirtualServer.BaseServer.write.String(builded_command);
                VirtualServer.BaseServer.write.Send(new SendInfo(VirtualServer.BaseServer.connections));
                
                BasePlayer.LocalPlayer.SetAdminStatus(false);
            }
            return builded_command;
        }

        public static string BuildCommand(string strCommand, params object[] args)
        {
            if ((args != null) && (args.Length != 0))
            {
                foreach (object obj2 in args)
                {
                    if (obj2 == null)
                    {
                        strCommand = strCommand + " \"\"";
                    }
                    else if (obj2 is float)
                    {
                        strCommand = string.Concat(strCommand, " ",
                            ((float) obj2).ToString("0.0", CultureInfo.InvariantCulture));
                    }
                    else if (obj2 is Color)
                    {
                        Color color = (Color)obj2;
                        object[] objArray1 = new object[] { color.r, color.g, color.b, color.a };
                        strCommand = strCommand + " " + QuoteSafe(string.Format("{0},{1},{2},{3}", objArray1));
                    }
                    else if (obj2 is Vector3)
                    {
                        Vector3 vector = (Vector3)obj2;
                        strCommand = string.Concat(strCommand, " ",
                            QuoteSafe( $"{vector.x.ToString("0.0", CultureInfo.InvariantCulture)},{vector.y.ToString("0.0", CultureInfo.InvariantCulture)},{vector.z.ToString("0.0", CultureInfo.InvariantCulture)}"
                                ));}
                    else
                    {
                        strCommand = strCommand + " " + QuoteSafe(obj2.ToString());
                    }
                }
            }
            return strCommand;
        }

        public static string QuoteSafe(string str)
        {
            char[] trimChars = new char[] { '\\' };
            str = str.Replace("\"", "\\\"").TrimEnd(trimChars);
            return ("\"" + str + "\"");
        }
        
        internal static string ToCSharpString(Vector3 pos)
        {
            return pos.x.ToString().Replace(',', '.') + " " + pos.y.ToString().Replace(',', '.') + " " + pos.z.ToString().Replace(',', '.');
        }
        
        internal static string ToCSharpString(float val)
        {
            return val.ToString().Replace(',', '.');
        }
    }
}