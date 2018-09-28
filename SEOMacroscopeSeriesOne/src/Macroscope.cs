﻿/*

	This file is part of SEOMacroscope.

	Copyright 2018 Jason Holland.

	The GitHub repository may be found at:

		https://github.com/nazuke/SEOMacroscope

	Foobar is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Foobar is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Runtime;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace SEOMacroscope
{

  /// <summary>
  /// Description of Macroscope.
  /// </summary>

  public class Macroscope
  {

    /**************************************************************************/

    // Override SuppressDebugMsg to enable/disable debugging output statically.
    public static bool SuppressStaticDebugMsg;

    // Set SuppressDebugMsg to enable/disable debugging output in object.
    public bool SuppressDebugMsg;

    protected static Dictionary<string, string> Memoize = new Dictionary<string, string>( 1024 );

    protected static bool ThrowInsufficientMemoryException;

    /**************************************************************************/

    static Macroscope ()
    {
      SuppressStaticDebugMsg = true;
      ThrowInsufficientMemoryException = false;
    }

    public Macroscope ()
    {
      this.SuppressDebugMsg = true;
    }

    /**************************************************************************/

    public static string GetVersion ()
    {
      string Location = Assembly.GetExecutingAssembly().Location;
      string Version = FileVersionInfo.GetVersionInfo( Location ).ProductVersion;
      return ( Version );
    }

    /** HTTP User Agent *******************************************************/

    public string UserAgentName ()
    {
#if DEBUG
      return ( "SEO-Macroscope-DEVELOPER-MODE" );
#else
      return ( "SEO-Macroscope" );
#endif
    }

    /** -------------------------------------------------------------------- **/

    public string UserAgentVersion ()
    {
#if DEBUG
      return ( "DEBUG" );
#else
      return( FileVersionInfo.GetVersionInfo( Assembly.GetExecutingAssembly().Location ).ProductVersion );
#endif
    }

    /** Global URL to Digest Routines *****************************************/

    public static int UrlToDigest ( string Url )
    {
      int hashed = Url.GetHashCode();
      return ( hashed );
    }

    /** Text Digest ***********************************************************/

    public static string GetStringDigest ( string Text )
    {

      string Digested = null;

      if( Macroscope.Memoize.ContainsKey( Text ) )
      {
        Digested = Macroscope.Memoize[Text];
      }
      else
      {

        HashAlgorithm Digest = HashAlgorithm.Create( "MD5" );
        byte[] BytesIn = Encoding.UTF8.GetBytes( Text );
        byte[] Hashed = Digest.ComputeHash( BytesIn );
        StringBuilder Buf = new StringBuilder();

        for( int i = 0 ; i < Hashed.Length ; i++ )
        {
          Buf.Append( Hashed[i].ToString( "X2" ) );
        }

        Digested = Buf.ToString();

        Macroscope.Memoize[Text] = Digested;

      }

      return ( Digested );
    }

    /** EXTERNAL BROWSER ******************************************************/

    public static void OpenUrlInBrowser ( string Url )
    {
      // The caller must catch any exceptions:
      Uri OpenUrl = null;
      OpenUrl = new Uri( Url );
      if( OpenUrl != null )
      {
        // FIXME: bug here when opening URL with query string
        Process.Start( OpenUrl.ToString() );
      }
    }

    /**************************************************************************/

    [Conditional( "DEVMODE" )]
    public static void DebugMsgStatic ( string Msg )
    {
      if( !SuppressStaticDebugMsg )
      {
        Debug.WriteLine( Msg );
      }
    }

    [Conditional( "DEVMODE" )]
    public void DebugMsg ( string Msg )
    {
      if( !SuppressDebugMsg )
      {
        Debug.WriteLine(
          string.Format(
            "TID:{0} :: {1} :: {2}",
            Thread.CurrentThread.ManagedThreadId,
            this.GetType(),
            Msg
          )
        );
      }
    }

    [Conditional( "DEVMODE" )]
    public void DebugMsgForced ( string Msg )
    {
      Debug.WriteLine(
        string.Format(
          "TID:{0} :: {1} :: {2}",
          Thread.CurrentThread.ManagedThreadId,
          this.GetType(),
          Msg
        )
      );
    }

    /**************************************************************************/

  }

}
