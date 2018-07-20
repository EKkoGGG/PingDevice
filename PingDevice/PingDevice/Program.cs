﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Net.NetworkInformation;
using System.Data;

namespace PingDevice
{
    class Program
    {
        static void Main(string[] args)
        {
            string conStr = "server =183.47.15.146; port=3306; user=root; password=joysw377136; database=db_jinruivms;";
            MySqlConnection connMySql = new MySqlConnection(conStr);
            string sql = "Select vpn_address,id From tb_device";
            MySqlDataAdapter sda = new MySqlDataAdapter(sql, connMySql);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            Console.WriteLine("PING设备中，请勿关闭!");
            while (true)
            {
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Ping ping = new Ping();
                        PingReply pingReply = ping.Send(dt.Rows[i]["vpn_address"].ToString());
                        if (pingReply.Status == IPStatus.Success)
                        {
                            //Console.WriteLine("当前在线，已PING通！");
                            connMySql.Open();
                            string comStr = "UPDATE tb_device SET `devStatus` = 1 WHERE id ='" + dt.Rows[i]["id"].ToString() + "' ";
                            MySqlCommand comm = new MySqlCommand(comStr, connMySql);
                            int count = comm.ExecuteNonQuery();
                            connMySql.Close();
                        }
                        else
                        {
                            //Console.WriteLine("不在线，PING不通！");
                            connMySql.Open();
                            string comStr = "UPDATE tb_device SET `devStatus` = 0 WHERE id ='" + dt.Rows[i]["id"].ToString() + "' ";
                            MySqlCommand comm = new MySqlCommand(comStr, connMySql);
                            int count = comm.ExecuteNonQuery();
                            connMySql.Close();
                        }
                    }
                }
                catch (Exception)
                {             
                    Console.WriteLine("错误，请检查设备地址是否填写正确");
                }

            }
        }
    }
}
