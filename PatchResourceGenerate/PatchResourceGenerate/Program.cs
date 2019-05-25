using Force.Crc32;
using System;
using System.Collections.Generic;
using System.IO;

namespace PatchResourceGenerate {
	class Program {
		static int GetInput( string text, int[] valid ) {
			int ret = 0;
			while( true ) {
				Console.WriteLine( text );
				var line = Console.ReadLine();
				if( int.TryParse( line, out ret ) ) {
					foreach( var item in valid ) {
						if( ret == item ) {
							return ret;
						}
					}

					Console.WriteLine( "输入的数字非法" );
				}
			}
		}

		static void Main( string[] args ) {
			var path = Environment.GetEnvironmentVariable( "H5_GANGSTER_RESOURCE" );
			if( string.IsNullOrEmpty( path ) ) {
				Console.WriteLine( @"请设置环境变量H5_GANGSTER_RESOURCE至你的svn\H5-Gangster\Egret\resource目录" );
				Console.ReadKey();
				return;
			}

			if( !Directory.Exists( path ) ) {
				Console.WriteLine( @"资源路径不存在：{0}", path );
				Console.ReadKey();
				return;
			}

			switch( GetInput( "请选择要执行的操作 1.生成初始版本 2.生成patch版本", new int[2] { 1, 2 } ) ) {
				case 1: {
						using( TextWriter writer = new StreamWriter( @".\init.txt" ) ) {
							var roots = Directory.GetDirectories( path );
							foreach( var root in roots ) {
								var files = Directory.GetFiles( root, "*.*", SearchOption.AllDirectories );
								foreach( var file in files ) {
									FileStream stream = new FileStream( file, FileMode.Open );
									byte[] buffer = new byte[stream.Length];
									stream.Read( buffer, 0, (int)stream.Length );
									stream.Close();
									uint crc32 = Crc32Algorithm.Compute( buffer );
									Console.WriteLine( "处理文件：{0}-{1}", Path.GetFileName( file ), crc32 );

									var fileRelatePath = file.Substring( path.Length + 1 );
									fileRelatePath = fileRelatePath.Replace( '\\', '/' );
									writer.WriteLine( "{0}:{1}", fileRelatePath, crc32 );
								}
							}
						}
					}
					break;
				case 2:
					if( !File.Exists( @".\init.txt" ) ) {
						Console.WriteLine( @"初始文件不存在请先执行1操作" );
						Console.ReadKey();
						return;
					}

					Dictionary<string, uint> initFileCrc = new Dictionary<string, uint>();
					using( TextReader reader = new StreamReader( @".\init.txt" ) ) {
						while( true ) {
							var line = reader.ReadLine();
							if( string.IsNullOrEmpty( line ) ) {
								break;
							}

							var parts = line.Split( ':' );
							var fileRelatePath = parts[0];
							uint crc32 = uint.Parse( parts[1] );

							initFileCrc.Add( fileRelatePath, crc32 );
						}
					}

					string patchFileName;
					while( true ) {
						Console.WriteLine( "请输入版本号例如：1.0" );
						patchFileName = Console.ReadLine();
						if( patchFileName.IndexOfAny( Path.GetInvalidFileNameChars() ) >= 0 ) {
							Console.WriteLine( "输入的名称非法！请重新输入" );
						}
						else {
							break;
						}
					}

					using( TextWriter patchWriter = new StreamWriter( @"./" + patchFileName + ".txt" ) ) {
						var roots = Directory.GetDirectories( path );
						foreach( var root in roots ) {
							var files = Directory.GetFiles( root, "*.*", SearchOption.AllDirectories );
							foreach( var file in files ) {
								FileStream stream = new FileStream( file, FileMode.Open );
								byte[] buffer = new byte[stream.Length];
								stream.Read( buffer, 0, (int)stream.Length );
								stream.Close();
								uint crc32 = Crc32Algorithm.Compute( buffer );
								var fileRelatePath = file.Substring( path.Length + 1 );
								fileRelatePath = fileRelatePath.Replace( '\\', '/' );
								if( initFileCrc.ContainsKey( fileRelatePath ) ) {
									if( initFileCrc[fileRelatePath] == crc32 ) {
										continue;
									}
								}

								Console.WriteLine( "新增Patch文件：{0}-{1}", Path.GetFileName( file ), crc32 );
								patchWriter.WriteLine( "{0}:{1}", fileRelatePath, crc32 );
							}
						}
					}
					Console.WriteLine( "生成Patch文件成功" );
					Console.ReadKey();
					break;
			}
		}
	}
}
