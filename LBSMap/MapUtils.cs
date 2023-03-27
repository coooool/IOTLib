using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace IOTLib
{
    /// 地图坐标转换实用类。
    /// 现常用的地图坐标系有GCJ-02和BD-09类型。 本类支持这两种坐标系互逆操作。
    /// BD-09类型：百度地图
    /// GCJ_02类型：谷歌、高德，腾讯
    /// 使用了百度地图或高德的坐标拾取器时。在百度的地图测试坐标时，该类会提供帮助。
    public class MapUtils
    {
        private const double pi = 3.14159265358979324;
        const double x_pi = 3.14159265358979324 * 3000.0 / 180.0;
        //地球半径，平均半径为6371km
        const double EARTH_RADIUS = 6378245; //6371; //6378137.0f;
        private const double ee = 0.00669342162296594323;

        /// <summary>
        /// 返回一个BD09坐标
        /// </summary>
        /// <param name="gcLng">经度</param>
        /// <param name="gcLat">纬度</param>
        /// <returns></returns>
        public static MapLocation GCJ02_BD09(double gcLng, double gcLat)
        {
            double x = gcLng, y = gcLat;

            double z = Mathf.Sqrt((float)(x * x + y * y) + 0.00002f * Mathf.Sin((float)(y * x_pi)));
            double theta = Mathf.Atan2((float)y, (float)x) + 0.000003 * Mathf.Cos((float)(x * x_pi));

            var bd_lon = z * Mathf.Cos((float)theta) + 0.0065;
            var bd_lat = z * Mathf.Sin((float)theta) + 0.006;

            return new MapLocation() { lat = bd_lat, lng = bd_lon };
        }

        /// <summary>
        /// BD09坐标转换为GCJ02
        /// 通常是将百度的坐标转换为高德、谷歌、腾讯系
        /// </summary>
        /// <param name="gdLat">纬度</param>
        /// <param name="bdLng">经度</param>
        /// <returns></returns>
        public static MapLocation BD09_GCJ02(double bdLng, double gdLat)
        {
            double x = bdLng - 0.0065, y = gdLat - 0.006;
            double z = Mathf.Sqrt((float)(x * x + y * y) - 0.00002f * Mathf.Sin((float)(y * x_pi)));
            double theta = Mathf.Atan2((float)y, (float)x) - 0.000003f * Mathf.Cos((float)(x * x_pi));

            var gg_lon = z * Mathf.Cos((float)theta);
            var gg_lat = z * Mathf.Sin((float)theta);

            return new MapLocation() { lat = gg_lat, lng = gg_lon };
        }

        private static double TransformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * pi) + 40.0 * Math.Sin(y / 3.0 * pi)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * pi) + 320 * Math.Sin(y * pi / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        private static double TransformLon(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * pi) + 40.0 * Math.Sin(x / 3.0 * pi)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * pi) + 300.0 * Math.Sin(x / 30.0 * pi)) * 2.0 / 3.0;
            return ret;
        }

        public static void Transform(double wgLat, double wgLon, double[] latlng)
        {
            if (OutOfChina(wgLat, wgLon))
            {
                latlng[0] = wgLat;
                latlng[1] = wgLon;
                return;
            }
            double dLat = TransformLat(wgLon - 105.0, wgLat - 35.0);
            double dLon = TransformLon(wgLon - 105.0, wgLat - 35.0);
            double radLat = wgLat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((EARTH_RADIUS * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLon = (dLon * 180.0) / (EARTH_RADIUS / sqrtMagic * Math.Cos(radLat) * pi);
            latlng[0] = wgLat + dLat;
            latlng[1] = wgLon + dLon;
        }

        private static bool OutOfChina(double lat, double lon)
        {
            if (lon < 72.004 || lon > 137.8347)
                return true;
            if (lat < 0.8293 || lat > 55.8271)
                return true;
            return false;
        }

        /// <summary>
        /// WGS84转GCJ02
        /// </summary>
        /// <param name="wgLat"></param>
        /// <param name="wgLon"></param>
        /// <returns></returns>
        public static MapLocation WGS84_to_GCJ02(double wgLat, double wgLon)
        {
            var point = new MapLocation();
            if (OutOfChina(wgLat, wgLon))
            {
                point.lat = wgLat;
                point.lng = wgLon;
                return point;
            }

            double dLat = TransformLat(wgLon - 105.0, wgLat - 35.0);
            double dLon = TransformLon(wgLon - 105.0, wgLat - 35.0);
            double radLat = wgLat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;

            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((EARTH_RADIUS * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLon = (dLon * 180.0) / (EARTH_RADIUS / sqrtMagic * Math.Cos(radLat) * pi);

            double lat = wgLat + dLat;
            double lon = wgLon + dLon;

            point.lat = lat;
            point.lng = lon;

            return point;
        }

        /// <summary>
        /// 根据一个LBS点展开四周的区域
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="distance">向外扩展多少经纬度</param>
        /// <returns>右上，左上，左下，右下</returns>
        public static Vector2D[] GetLBSPointRect(double lng, double lat, double distance)
        {
            double dlng = 2 * Mathf.Asin(Mathf.Sin((float)(distance / (2 * EARTH_RADIUS) / Mathf.Cos((float)(Mathf.Deg2Rad * lat)))));
            dlng = Mathf.Rad2Deg * dlng;
            double dlat = distance / EARTH_RADIUS;
            dlat = Mathf.Rad2Deg * dlat;

            return new[]
            {
             new Vector2D((lat + dlat), (lng + dlng)),
             new Vector2D((lat + dlat), (lng - dlng)),
             new Vector2D((lat - dlat), (lng - dlng)),
             new Vector2D(lat - dlat, (lng + dlng)),
          };
        }
    }
}