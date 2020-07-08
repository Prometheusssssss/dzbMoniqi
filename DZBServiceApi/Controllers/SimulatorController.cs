using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace WechatRouteApi.Controllers
{

    [RoutePrefix("api/simulator")]
    public class SimulatorController : ApiController
    {
        //#region X.成员方法[使用data.Json获取数据  可扩展性强]
        ///// <summary>
        ///// 使用data.Json获取数据  可扩展性强
        ///// </summary>
        //[HttpPost]
        //[Route("dzmnq")]
        //public object Post([FromBody]JToken json)
        //{
        //    var fileName = "data.json";
        //    string jsonfile = System.Web.HttpContext.Current.Server.MapPath("/Config/" + fileName);
        //    try
        //    {
        //        using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
        //        {
        //            using (JsonTextReader reader = new JsonTextReader(file))
        //            {
        //                var jsonDy = json as dynamic;
        //                var count = jsonDy.Count;
        //                for (int i = 0; i < count; i++)
        //                {
        //                    var data = jsonDy[i];
        //                    string type = data.TYPE;
        //                    string level = data.LEVEL;
        //                    JObject o = (JObject)JToken.ReadFrom(reader);
        //                    var value = o["JSON"].Where(p => p["NAME"].ToString() == type).SingleOrDefault();
        //                    var details = value["DETAILS"];
        //                    var levelData = details.Where(p => p["LEVEL"].ToString() == level).SingleOrDefault();
        //                    //获取数据 
        //                    var hp = levelData["HP"].ToString();
        //                    string appId = value["HP"].ToString();
        //                }
        //                return null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonConvert.SerializeObject(new SendMsgModel()
        //        {
        //            errcode = 1,
        //            errmsg = ex.Message
        //        });
        //    }
        //}
        //#endregion

        #region X.成员方法[计算]
        /// <summary>
        /// 使用Model Data获取数据
        /// </summary>
        [HttpPost]
        [Route("dzmnq")]
        public object Post([FromBody]JToken json)
        {
            var jsonDy = json as dynamic;
            var dataList = DataClass.X.dataModels;
            var jsonList = new List<JsonModel>();
            var count = jsonDy.Count;
            //获取需要计算的绝学
            for (int i = 0; i < count; i++)
            {
                var data = jsonDy[i];
                var jsonModel = new JsonModel()
                {
                    CATEGORY = data.CATEGORY,
                    NAME = data.NAME,
                    SEQ_NO = data.SEQ_NO,
                    LEVEL = data.LEVEL,
                    BURROW = data.BURROW
                };
                jsonList.Add(jsonModel);
            }
            var backModel = new ResultModel();
            //判断一套二套绝学
            var firstJsonList = jsonList.Where(p => p.CATEGORY == 1);
            var firstNameList = firstJsonList.Select(p => p.NAME).ToList();
            var sencondJsonList = jsonList.Where(p => p.CATEGORY == 2);
            //生成第二套绝学名称
            var sencondNameList = sencondJsonList.Select(p => p.NAME).ToList();

            #region 第一套绝学
            //判断羁绊
            foreach (var item in firstJsonList)
            {
                var percentage = 0m;
                if (item.SEQ_NO == 1)
                    percentage = 1;
                else if (item.SEQ_NO == 2)
                    percentage = 0.7m;
                else if (item.SEQ_NO == 3)
                    percentage = 0.5m;
                else if (item.SEQ_NO == 4)
                    percentage = 0.3m;

                //获取数据
                var basicData = dataList.Where(p => p.NAME == item.NAME).SingleOrDefault();
                if (basicData != null)
                {
                    //获取等级数据
                    var levelData = basicData.DETAILS.Where(p => p.LEVEL == item.LEVEL).SingleOrDefault();
                    if (levelData != null)
                    {
                        //创建数据
                        var result = new ResultModel()
                        {
                            QX = levelData.QX,
                            SB = levelData.SB,
                            GJ = levelData.GJ,
                            MZ = levelData.MZ,
                            PF = levelData.PF,
                            KB = levelData.KB,
                            BJ = levelData.BJ,
                            GD = levelData.GD,
                            FY = 0,
                            ZXW = levelData.ZXW,
                            ZZL = 0
                        };

                        #region 红色绝学自带2000战力
                        //独孤九剑
                        if (item.NAME == "dgjj")
                        {
                            //红色绝学自带2000战力
                            result.ZZL = 2000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("xxdf");
                            fettersNameList.Add("yjj");
                            fettersNameList.Add("tjjf");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "xxdf")
                                {
                                    result.GJ = result.GJ * 1.3m;
                                }
                                else if (fetters == "yjj")
                                {
                                    result.MZ = result.MZ * 1.3m;
                                }
                                else if (fetters == "tjjf")
                                {
                                    result.PF = result.PF * 1.3m;
                                }
                            }
                        }
                        //吸星大法
                        else if (item.NAME == "xxdf")
                        {
                            //红色绝学自带2000战力
                            result.ZZL = 2000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("dgjj");
                            fettersNameList.Add("yjj");
                            fettersNameList.Add("shl");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "dgjj")
                                {
                                    result.GJ = result.GJ * 1.3m;
                                }
                                else if (fetters == "yjj")
                                {
                                    result.BJ = result.BJ * 1.3m;
                                }
                                else if (fetters == "shl")
                                {
                                    result.KB = result.KB * 1.3m;
                                }
                            }
                        }
                        //易筋经
                        else if (item.NAME == "yjj")
                        {
                            //红色绝学自带2000战力
                            result.ZZL = 2000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("dgjj");
                            fettersNameList.Add("xxdf");
                            fettersNameList.Add("ssf");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "dgjj")
                                {
                                    result.GJ = result.GJ * 1.3m;
                                }
                                else if (fetters == "xxdf")
                                {
                                    result.QX = result.QX * 1.3m;
                                }
                                else if (fetters == "ssf")
                                {
                                    result.GD = result.GD * 1.3m;
                                }
                            }
                        }
                        //笑傲江湖曲
                        else if (item.NAME == "xajhq")
                        {
                            //红色绝学自带2000战力
                            result.ZZL = 2000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("yjj");
                            fettersNameList.Add("xxdf");
                            fettersNameList.Add("qxpsz");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "yjj")
                                {
                                    result.QX = result.QX * 1.3m;
                                }
                                else if (fetters == "xxdf")
                                {
                                    result.BJ = result.BJ * 1.3m;
                                }
                                else if (fetters == "qxpsz")
                                {
                                    result.GD = result.GD * 1.3m;
                                }
                            }
                        }
                        //辟邪剑法
                        else if (item.NAME == "pxjf")
                        {
                            //红色绝学自带2000战力
                            result.ZZL = 2000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("dgjj");
                            fettersNameList.Add("yjj");
                            fettersNameList.Add("zxsg");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "dgjj")
                                {
                                    result.GJ = result.GJ * 1.3m;
                                }
                                else if (fetters == "yjj")
                                {
                                    result.PF = result.PF * 1.3m;
                                }
                                else if (fetters == "zxsg")
                                {
                                    result.SB = result.SB * 1.3m;
                                }
                            }
                        }
                        #endregion

                        #region 黄色绝学自带1000战力
                        //化功大法
                        else if (item.NAME == "hgdf")
                        {
                            //黄色绝学自带1000战力
                            result.ZZL = 1000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("zxsg");
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "zxsg")
                                {
                                    result.PF = result.PF * 1.2m;
                                }
                                else if (fetters == "hbzq")
                                {
                                    result.SB = result.SB * 1.2m;
                                }
                            }
                        }
                        //清心普善咒
                        else if (item.NAME == "qxpsz")
                        {
                            //黄色绝学自带1000战力
                            result.ZZL = 1000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("whpl");
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "whpl")
                                {
                                    result.QX = result.QX * 1.2m;
                                }
                                else if (fetters == "hbzq")
                                {
                                    result.KB = result.KB * 1.2m;
                                }
                            }
                        }
                        //生死符
                        else if (item.NAME == "ssf")
                        {
                            //黄色绝学自带1000战力
                            result.ZZL = 1000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("shl");
                            fettersNameList.Add("qxwjj");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "shl")
                                {
                                    result.PF = result.PF * 1.2m;
                                }
                                else if (fetters == "qxwjj")
                                {
                                    result.BJ = result.BJ * 1.2m;
                                }
                            }
                        }
                        //圣火令
                        else if (item.NAME == "shl")
                        {
                            //黄色绝学自带1000战力
                            result.ZZL = 1000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("tjjf");
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "tjjf")
                                {
                                    result.BJ = result.BJ * 1.2m;
                                }
                                else if (fetters == "hbzq")
                                {
                                    result.KB = result.KB * 1.2m;
                                }
                            }
                        }
                        //紫霞神功
                        else if (item.NAME == "zxsg")
                        {
                            //黄色绝学自带1000战力
                            result.ZZL = 1000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("ssf");
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "ssf")
                                {
                                    result.QX = result.QX * 1.2m;
                                }
                                else if (fetters == "hbzq")
                                {
                                    result.KB = result.KB * 1.2m;
                                }
                            }
                        }
                        //太极剑法
                        else if (item.NAME == "tjjf")
                        {
                            //黄色绝学自带1000战力
                            result.ZZL = 1000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("ssf");
                            fettersNameList.Add("qxwjj");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "ssf")
                                {
                                    result.PF = result.PF * 1.2m;
                                }
                                else if (fetters == "qxwjj")
                                {
                                    result.GD = result.GD * 1.2m;
                                }
                            }
                        }
                        #endregion

                        #region 紫色绝学自带500战力
                        //寒冰真气
                        else if (item.NAME == "hbzq")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("qxwjj");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "qxwjj")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        //黑血神针
                        else if (item.NAME == "hxsz")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "hbzq")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        //七弦无形剑
                        else if (item.NAME == "qxwjj")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "hbzq")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        //千相正阳掌
                        else if (item.NAME == "qxzyz")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "hbzq")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        //太岳三青峰
                        else if (item.NAME == "tysqf")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "hbzq")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        //万花飘零
                        else if (item.NAME == "whpl")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "hbzq")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        //真武七截剑
                        else if (item.NAME == "zwqjj")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "hbzq")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        #endregion


                        //计算战力 1气血=1/6战力；1攻击=5战力；其他属性=4战力
                        result.ZZL += result.QX / 6 + result.GJ * 5 + (result.MZ + result.PF + result.KB + result.BJ + result.SB + result.GD) * 4;

                        #region 钻研等级
                        var burrowNum = 0;
                        //判断绝学颜色 增加等级
                        //红色
                        if (item.NAME == "dgjj" || item.NAME == "xxdf" || item.NAME == "yjj" || item.NAME == "xajhq" || item.NAME == "pxjf")
                        {
                            //新增4个属性的运算单独运算

                            if (item.BURROW == 1)
                            {
                                //运算新增数据
                                result.QX += 8883;
                                result.MZ += 369;
                                result.GJ += 295;
                                result.FY += 885;
                                burrowNum = 9804;
                            }
                            else if (item.BURROW == 2)
                            {
                                result.QX += 18277;
                                result.MZ += 759;
                                result.GJ += 607;
                                result.FY += 1821;
                                burrowNum = 22288;
                            }
                            else if (item.BURROW == 3)
                            {
                                result.QX += 29550;
                                result.MZ += 1226;
                                result.GJ += 981;
                                result.FY += 2944;
                                burrowNum = 37261;
                            }
                            else if (item.BURROW == 4)
                            {
                                result.QX += 44581;
                                result.MZ += 1850;
                                result.GJ += 1480;
                                result.FY += 4442;
                                burrowNum = 57235;
                            }
                            else if (item.BURROW == 5)
                            {
                                result.QX += 61416;
                                result.MZ += 2549;
                                result.GJ += 2039;
                                result.FY += 6119;
                                burrowNum = 79605;
                            }
                        }
                        //黄色
                        else if (item.NAME == "hgdf" || item.NAME == "qxpsz" || item.NAME == "ssf" || item.NAME == "shl" || item.NAME == "zxsg" || item.NAME == "tjjf")
                        {
                            if (item.BURROW == 1)
                            {
                                result.QX += 4441;
                                result.MZ += 184;
                                result.GJ += 147;
                                result.FY += 443;
                                burrowNum = 4899;
                            }
                            else if (item.BURROW == 2)
                            {
                                result.QX += 9139;
                                result.MZ += 379;
                                result.GJ += 303;
                                result.FY += 911;
                                burrowNum = 11145;
                            }
                            else if (item.BURROW == 3)
                            {
                                result.QX += 14774;
                                result.MZ += 613;
                                result.GJ += 491;
                                result.FY += 1472;
                                burrowNum = 18635;
                            }
                            else if (item.BURROW == 4)
                            {
                                result.QX += 22291;
                                result.MZ += 925;
                                result.GJ += 740;
                                result.FY += 2221;
                                burrowNum = 28621;
                            }
                            else if (item.BURROW == 5)
                            {
                                result.QX += 30708;
                                result.MZ += 1274;
                                result.GJ += 1020;
                                result.FY += 3060;
                                burrowNum = 39807;
                            }
                        }
                        //紫色
                        else if (item.NAME == "hbzq" || item.NAME == "hxsz" || item.NAME == "qxwjj" || item.NAME == "qxzyz" || item.NAME == "tysqf" || item.NAME == "whpl" || item.NAME == "zwqjj")
                        {
                            if (item.BURROW == 1)
                            {
                                result.QX += 2220;
                                result.MZ += 93;
                                result.GJ += 74;
                                result.FY += 221;
                                burrowNum = 2450;
                            }
                            else if (item.BURROW == 2)
                            {
                                result.QX += 4569;
                                result.MZ += 190;
                                result.GJ += 152;
                                result.FY += 455;
                                burrowNum = 5574;
                            }
                            else if (item.BURROW == 3)
                            {
                                result.QX += 7387;
                                result.MZ += 307;
                                result.GJ += 245;
                                result.FY += 736;
                                burrowNum = 9316;
                            }
                            else if (item.BURROW == 4)
                            {
                                result.QX += 11145;
                                result.MZ += 463;
                                result.GJ += 370;
                                result.FY += 1110;
                                burrowNum = 14310;
                            }
                            else if (item.BURROW == 5)
                            {
                                result.QX += 15354;
                                result.MZ += 637;
                                result.GJ += 510;
                                result.FY += 1530;
                                burrowNum = 19902;
                            }
                        }
                        #endregion

                        result.ZZL += burrowNum;
                        //增加钻研战力
                        result.ZZL = result.ZZL * percentage;
                        //累加resultModel
                        backModel.QX += result.QX * percentage;
                        backModel.SB += result.SB * percentage;
                        backModel.GJ += result.GJ * percentage;
                        backModel.MZ += result.MZ * percentage;
                        backModel.PF += result.PF * percentage;
                        backModel.KB += result.KB * percentage;
                        backModel.BJ += result.BJ * percentage;
                        backModel.GD += result.GD * percentage;
                        backModel.FY += result.FY * percentage;
                        backModel.ZXW += result.ZXW;
                        backModel.ZZL += result.ZZL;
                    }
                }
            }
            #endregion

            #region 第二套绝学
            //判断羁绊
            foreach (var item in sencondJsonList)
            {
                var percentage = 0.0m;
                if (item.SEQ_NO == 1)
                    percentage = 0.7m;
                else if (item.SEQ_NO == 2)
                    percentage = 0.5m;
                else if (item.SEQ_NO == 3)
                    percentage = 0.3m;
                else if (item.SEQ_NO == 4)
                    percentage = 0.2m;

                //获取数据
                var basicData = dataList.Where(p => p.NAME == item.NAME).SingleOrDefault();
                if (basicData != null)
                {
                    //获取等级数据
                    var levelData = basicData.DETAILS.Where(p => p.LEVEL == item.LEVEL).SingleOrDefault();
                    if (levelData != null)
                    {
                        //创建数据
                        var result = new ResultModel()
                        {
                            QX = levelData.QX,
                            SB = levelData.SB,
                            GJ = levelData.GJ,
                            MZ = levelData.MZ,
                            PF = levelData.PF,
                            KB = levelData.KB,
                            BJ = levelData.BJ,
                            GD = levelData.GD,
                            FY = 0,
                            ZXW = levelData.ZXW,
                            ZZL = 0
                        };

                        #region 红色绝学自带2000战力
                        //独孤九剑
                        if (item.NAME == "dgjj")
                        {
                            //红色绝学自带2000战力
                            result.ZZL = 2000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("xxdf");
                            fettersNameList.Add("yjj");
                            fettersNameList.Add("tjjf");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "xxdf")
                                {
                                    result.GJ = result.GJ * 1.3m;
                                }
                                else if (fetters == "yjj")
                                {
                                    result.MZ = result.MZ * 1.3m;
                                }
                                else if (fetters == "tjjf")
                                {
                                    result.PF = result.PF * 1.3m;
                                }
                            }
                        }
                        //吸星大法
                        else if (item.NAME == "xxdf")
                        {
                            //红色绝学自带2000战力
                            result.ZZL = 2000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("dgjj");
                            fettersNameList.Add("yjj");
                            fettersNameList.Add("shl");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "dgjj")
                                {
                                    result.GJ = result.GJ * 1.3m;
                                }
                                else if (fetters == "yjj")
                                {
                                    result.BJ = result.BJ * 1.3m;
                                }
                                else if (fetters == "shl")
                                {
                                    result.KB = result.KB * 1.3m;
                                }
                            }
                        }
                        //易筋经
                        else if (item.NAME == "yjj")
                        {
                            //红色绝学自带2000战力
                            result.ZZL = 2000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("dgjj");
                            fettersNameList.Add("xxdf");
                            fettersNameList.Add("ssf");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "dgjj")
                                {
                                    result.GJ = result.GJ * 1.3m;
                                }
                                else if (fetters == "xxdf")
                                {
                                    result.QX = result.QX * 1.3m;
                                }
                                else if (fetters == "ssf")
                                {
                                    result.GD = result.GD * 1.3m;
                                }
                            }
                        }
                        //笑傲江湖曲
                        else if (item.NAME == "xajhq")
                        {
                            //红色绝学自带2000战力
                            result.ZZL = 2000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("yjj");
                            fettersNameList.Add("xxdf");
                            fettersNameList.Add("qxpsz");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "yjj")
                                {
                                    result.QX = result.QX * 1.3m;
                                }
                                else if (fetters == "xxdf")
                                {
                                    result.BJ = result.BJ * 1.3m;
                                }
                                else if (fetters == "qxpsz")
                                {
                                    result.GD = result.GD * 1.3m;
                                }
                            }
                        }
                        //辟邪剑法
                        else if (item.NAME == "pxjf")
                        {
                            //红色绝学自带2000战力
                            result.ZZL = 2000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("dgjj");
                            fettersNameList.Add("yjj");
                            fettersNameList.Add("zxsg");
                            //寻找羁绊，判断加成
                            var fettersList = firstNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "dgjj")
                                {
                                    result.GJ = result.GJ * 1.3m;
                                }
                                else if (fetters == "yjj")
                                {
                                    result.PF = result.PF * 1.3m;
                                }
                                else if (fetters == "zxsg")
                                {
                                    result.SB = result.SB * 1.3m;
                                }
                            }
                        }
                        #endregion

                        #region 黄色绝学自带1000战力
                        //化功大法
                        else if (item.NAME == "hgdf")
                        {
                            //黄色绝学自带1000战力
                            result.ZZL = 1000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("zxsg");
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "zxsg")
                                {
                                    result.PF = result.PF * 1.2m;
                                }
                                else if (fetters == "hbzq")
                                {
                                    result.SB = result.SB * 1.2m;
                                }
                            }
                        }
                        //清心普善咒
                        else if (item.NAME == "qxpsz")
                        {
                            //黄色绝学自带1000战力
                            result.ZZL = 1000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("whpl");
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "whpl")
                                {
                                    result.QX = result.QX * 1.2m;
                                }
                                else if (fetters == "hbzq")
                                {
                                    result.KB = result.KB * 1.2m;
                                }
                            }
                        }
                        //生死符
                        else if (item.NAME == "ssf")
                        {
                            //黄色绝学自带1000战力
                            result.ZZL = 1000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("shl");
                            fettersNameList.Add("qxwjj");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "shl")
                                {
                                    result.PF = result.PF * 1.2m;
                                }
                                else if (fetters == "qxwjj")
                                {
                                    result.BJ = result.BJ * 1.2m;
                                }
                            }
                        }
                        //圣火令
                        else if (item.NAME == "shl")
                        {
                            //黄色绝学自带1000战力
                            result.ZZL = 1000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("tjjf");
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "tjjf")
                                {
                                    result.BJ = result.BJ * 1.2m;
                                }
                                else if (fetters == "hbzq")
                                {
                                    result.KB = result.KB * 1.2m;
                                }
                            }
                        }
                        //紫霞神功
                        else if (item.NAME == "zxsg")
                        {
                            //黄色绝学自带1000战力
                            result.ZZL = 1000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("ssf");
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "ssf")
                                {
                                    result.QX = result.QX * 1.2m;
                                }
                                else if (fetters == "hbzq")
                                {
                                    result.KB = result.KB * 1.2m;
                                }
                            }
                        }
                        //太极剑法
                        else if (item.NAME == "tjjf")
                        {
                            //黄色绝学自带1000战力
                            result.ZZL = 1000;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("ssf");
                            fettersNameList.Add("qxwjj");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "ssf")
                                {
                                    result.PF = result.PF * 1.2m;
                                }
                                else if (fetters == "qxwjj")
                                {
                                    result.GD = result.GD * 1.2m;
                                }
                            }
                        }
                        #endregion

                        #region 紫色绝学自带500战力
                        //寒冰真气
                        else if (item.NAME == "hbzq")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("qxwjj");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "qxwjj")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        //黑血神针
                        else if (item.NAME == "hxsz")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "hbzq")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        //七弦无形剑
                        else if (item.NAME == "qxwjj")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "hbzq")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        //千相正阳掌
                        else if (item.NAME == "qxzyz")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "hbzq")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        //太岳三青峰
                        else if (item.NAME == "tysqf")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "hbzq")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        //万花飘零
                        else if (item.NAME == "whpl")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "hbzq")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        //真武七截剑
                        else if (item.NAME == "zwqjj")
                        {
                            //紫色绝学自带500战力
                            result.ZZL = 500;
                            //构造羁绊List
                            var fettersNameList = new List<string>();
                            fettersNameList.Add("hbzq");
                            //寻找羁绊，判断加成
                            var fettersList = sencondNameList.Where(p => fettersNameList.Contains(p));

                            foreach (var fetters in fettersList)
                            {
                                if (fetters == "hbzq")
                                {
                                    result.GJ = result.GJ * 1.1m;
                                }
                            }
                        }
                        #endregion

                        //计算战力 1气血=1/6战力；1攻击=5战力；其他属性=4战力
                        result.ZZL += result.QX / 6 + result.GJ * 5 + (result.MZ + result.PF + result.KB + result.BJ + result.SB + result.GD) * 4;

                        #region 钻研等级
                        var burrowNum = 0;
                        //判断绝学颜色 增加等级
                        //红色
                        if (item.NAME == "dgjj" || item.NAME == "xxdf" || item.NAME == "yjj" || item.NAME == "xajhq" || item.NAME == "pxjf")
                        {
                            //新增4个属性的运算单独运算

                            if (item.BURROW == 1)
                            {
                                //运算新增数据
                                result.QX += 8883;
                                result.MZ += 369;
                                result.GJ += 295;
                                result.FY += 885;

                                burrowNum = 9804;
                            }
                            else if (item.BURROW == 2)
                            {
                                result.QX += 18277;
                                result.MZ += 759;
                                result.GJ += 607;
                                result.FY += 1821;
                                burrowNum = 22288;
                            }
                            else if (item.BURROW == 3)
                            {
                                result.QX += 29550;
                                result.MZ += 1226;
                                result.GJ += 981;
                                result.FY += 2944;
                                burrowNum = 37261;
                            }
                            else if (item.BURROW == 4)
                            {
                                result.QX += 44581;
                                result.MZ += 1850;
                                result.GJ += 1480;
                                result.FY += 4442;
                                burrowNum = 57235;
                            }
                            else if (item.BURROW == 5)
                            {
                                result.QX += 61416;
                                result.MZ += 2549;
                                result.GJ += 2039;
                                result.FY += 6119;
                                burrowNum = 79605;
                            }
                        }
                        //黄色
                        else if (item.NAME == "hgdf" || item.NAME == "qxpsz" || item.NAME == "ssf" || item.NAME == "shl" || item.NAME == "zxsg" || item.NAME == "tjjf")
                        {
                            if (item.BURROW == 1)
                            {
                                result.QX += 4441;
                                result.MZ += 184;
                                result.GJ += 147;
                                result.FY += 443;
                                burrowNum = 4899;
                            }
                            else if (item.BURROW == 2)
                            {
                                result.QX += 9139;
                                result.MZ += 379;
                                result.GJ += 303;
                                result.FY += 911;
                                burrowNum = 11145;
                            }
                            else if (item.BURROW == 3)
                            {
                                result.QX += 14774;
                                result.MZ += 613;
                                result.GJ += 491;
                                result.FY += 1472;
                                burrowNum = 18635;
                            }
                            else if (item.BURROW == 4)
                            {
                                result.QX += 22291;
                                result.MZ += 925;
                                result.GJ += 740;
                                result.FY += 2221;
                                burrowNum = 28621;
                            }
                            else if (item.BURROW == 5)
                            {
                                result.QX += 30708;
                                result.MZ += 1274;
                                result.GJ += 1020;
                                result.FY += 3060;
                                burrowNum = 39807;
                            }
                        }
                        //紫色
                        else if (item.NAME == "hbzq" || item.NAME == "hxsz" || item.NAME == "qxwjj" || item.NAME == "qxzyz" || item.NAME == "tysqf" || item.NAME == "whpl" || item.NAME == "zwqjj")
                        {
                            if (item.BURROW == 1)
                            {
                                result.QX += 2220;
                                result.MZ += 93;
                                result.GJ += 74;
                                result.FY += 221;
                                burrowNum = 2450;
                            }
                            else if (item.BURROW == 2)
                            {
                                result.QX += 4569;
                                result.MZ += 190;
                                result.GJ += 152;
                                result.FY += 455;
                                burrowNum = 5574;
                            }
                            else if (item.BURROW == 3)
                            {
                                result.QX += 7387;
                                result.MZ += 307;
                                result.GJ += 245;
                                result.FY += 736;
                                burrowNum = 9316;
                            }
                            else if (item.BURROW == 4)
                            {
                                result.QX += 11145;
                                result.MZ += 463;
                                result.GJ += 370;
                                result.FY += 1110;
                                burrowNum = 14310;
                            }
                            else if (item.BURROW == 5)
                            {
                                result.QX += 15354;
                                result.MZ += 637;
                                result.GJ += 510;
                                result.FY += 1530;
                                burrowNum = 19902;
                            }
                        }
                        #endregion

                        //增加钻研战力
                        result.ZZL += burrowNum;
                        result.ZZL = result.ZZL * percentage;
                        //累加resultModel
                        backModel.QX += result.QX * percentage;
                        backModel.SB += result.SB * percentage;
                        backModel.GJ += result.GJ * percentage;
                        backModel.MZ += result.MZ * percentage;
                        backModel.PF += result.PF * percentage;
                        backModel.KB += result.KB * percentage;
                        backModel.BJ += result.BJ * percentage;
                        backModel.GD += result.GD * percentage;
                        backModel.ZXW += result.ZXW;
                        backModel.ZZL += result.ZZL;
                    }
                }
            }
            #endregion

            backModel.QX = Math.Round(backModel.QX);
            backModel.SB = Math.Round(backModel.SB);
            backModel.GJ = Math.Round(backModel.GJ);
            backModel.MZ = Math.Round(backModel.MZ);
            backModel.PF = Math.Round(backModel.PF);
            backModel.KB = Math.Round(backModel.KB);
            backModel.BJ = Math.Round(backModel.BJ);
            backModel.GD = Math.Round(backModel.GD);
            backModel.FY = Math.Round(backModel.FY);
            backModel.ZXW = Math.Round(backModel.ZXW);
            backModel.ZZL = Math.Round(backModel.ZZL);
            return backModel;
        }
        #endregion

        /// <summary>
        /// 使用Model Data获取数据
        /// </summary>
        [HttpGet]
        [Route("test/{type}")]
        public string GetTest(string type)
        {
            return type;
        }

        public class JsonModel
        {
            public int CATEGORY { get; set; }
            public string NAME { get; set; }
            public int SEQ_NO { get; set; }
            public int LEVEL { get; set; }
            public int BURROW { get; set; }
        }

        public class ResultModel
        {
            public decimal QX { get; set; }
            public decimal SB { get; set; }
            public decimal GJ { get; set; }
            public decimal MZ { get; set; }
            public decimal PF { get; set; }
            public decimal KB { get; set; }
            public decimal BJ { get; set; }
            public decimal GD { get; set; }
            public decimal FY { get; set; }
            public decimal ZXW { get; set; }
            public decimal ZZL { get; set; }

        }
    }
}
