using System;
using System.IO;
using System.Net;
using System.Text;

namespace JsonTest
{
	class StructToJson
	{
		public static string Convert(Object obj)
		{
			string s = "";
			Type t = obj.GetType();

			// Primitive and String
			if (t.IsPrimitive || t==typeof(String))
			{
				return "'"+obj.ToString()+"'";
			}

			// Array
			if (t.IsArray)
			{
				Array array = (Array)obj;
				for (int i=0; i<array.Length; ++i)
				{
					if (i!=0)
						s += ",";
					s += Convert(array.GetValue(i));
				}
				return "["+s+"]";
			}

			// Field
			foreach (var inf in t.GetFields())
			{
				if (s!="")
					s += ",";

			//	Console.WriteLine("{0}", inf.Name);
				var value = inf.GetValue(obj);
				s += "'"+inf.Name+"':"+Convert(value);
			}

			return "{"+s+"}";
		}
	}

	struct Data
	{
		public struct User
		{
			public string username;
			public string player_id;
		}
		public struct Record
		{
			public string movie_path;
			public float movie_second;
			public int movie_frames;
			public string register_date;
			public string data;
			public string[] partner_id;
			public string comment;
			public string pattern;
			public string sound;
			public string background;
		}
		public struct Image
		{
			public string filename;
			public string ext;
			public string mime;
			public int width;
			public int height;
			public string data;
		};
		public User user;
		public Record record;
		public Image[] image;
	}

	class Program
	{
		static string post_uri =
			"http://www4235ui.sakura.ne.jp/ST_Player/api/playDataSaveDebug";
	
		static string post(Data data)
		{
			string postData = "json="+StructToJson.Convert(data)+"";
#if false
			postData =
				@"json={'User':"+
				@"{'username':'usr','player_id':'ABCD'},"+
				@"'Record':{'movie_path':'','movie_length':'',"+
				@"'register_date':'','data':'',"+
				@"'partner_id':['EFGH'],'tags':'tag,tag2',"+
				@"'comment':'','pattern':'ptn','sound':'snd'}"+
				@"}";
#endif			
			// Single quote to Double quote
			postData = postData.Replace('\'', '"');
			
			byte[] postDataBytes = Encoding.UTF8.GetBytes(postData);
			
			foreach (byte b in postDataBytes)
			{
				Console.Write("{0}", (char)b);
			}
			Console.Write("(end)");
			Console.WriteLine();
			Console.WriteLine();


			var req = WebRequest.Create(post_uri);
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = postDataBytes.Length;

			var reqStream = req.GetRequestStream();
			reqStream.Write(postDataBytes, 0, postDataBytes.Length);
			reqStream.Close();

			var res = req.GetResponse();
			var sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
			string result = sr.ReadToEnd();
			sr.Close();
			return result;
		}

		static void Main(string[] args)
		{
			string player_id = "P00000000PC9801";

			Data data = new Data();
			data.user.username = "みなんこ";
			data.user.player_id = player_id;
			data.record.movie_path = "movpath";
			data.record.movie_second = 229/30.0f;
			data.record.movie_frames = 229;
			data.record.register_date = "2014-01-01 00:11:22";
			data.record.data = "this is data";
			data.record.partner_id = new string[]{"partner1","partner2","partner3"};
			data.record.comment = "こめん";
			data.record.pattern = "smorky";
			data.record.sound = "machine";
			data.record.background = "grass";

			const int IMAGES = 6;
			data.image = new Data.Image[IMAGES];
			for (int i=0; i<IMAGES; ++i)
			{
				data.image[i].filename = player_id + "-" + i.ToString();
				data.image[i].ext = "jpg";
				data.image[i].mime = "image/jpeg";
				data.image[i].width = 640;
				data.image[i].height = 480;
				data.image[i].data = "<binary>";
			}

			string s = post(data);
			Console.WriteLine(s);

			Console.ReadLine();
		}
	}
}
