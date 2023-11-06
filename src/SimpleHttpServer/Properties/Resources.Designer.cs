﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SimpleHttpServer.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SimpleHttpServer.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Representation: Content-Type, Content-Encoding, Content-Language, Content-Location
        ///Payload: Content-Length, Content-Range, Trailer, Transfer-Encoding.
        /// </summary>
        internal static string HeaderGroupDetails {
            get {
                return ResourceManager.GetString("HeaderGroupDetails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html lang=&quot;en&quot;&gt;
        ///
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width&quot;&gt;
        ///    &lt;style&gt;
        ///        body {
        ///            text-align: center;
        ///        }
        ///    &lt;/style&gt;
        ///    &lt;title&gt;400 Bad Request&lt;/title&gt;
        ///&lt;/head&gt;
        ///
        ///&lt;body&gt;
        ///    &lt;h1&gt;400 Bad Request&lt;/h1&gt;
        ///    &lt;p&gt;Your client sent an illegal request&lt;/p&gt;
        ///    &lt;hr&gt;
        ///    &lt;p&gt;Simple HTTP server&lt;/p&gt;
        ///&lt;/body&gt;
        ///
        ///&lt;/html&gt;.
        /// </summary>
        internal static string ResponsePage400 {
            get {
                return ResourceManager.GetString("ResponsePage400", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html lang=&quot;en&quot;&gt;
        ///
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width&quot;&gt;
        ///    &lt;style&gt;
        ///        body {
        ///            text-align: center;
        ///        }
        ///    &lt;/style&gt;
        ///    &lt;title&gt;401 Unauthorized&lt;/title&gt;
        ///&lt;/head&gt;
        ///
        ///&lt;body&gt;
        ///    &lt;h1&gt;401 Unauthorized&lt;/h1&gt;
        ///    &lt;p&gt;You don&apos;t have permission to access this resource using the credentials you provided&lt;/p&gt;
        ///    &lt;hr&gt;
        ///    &lt;p&gt;Simple HTTP server&lt;/p&gt;
        ///&lt;/body&gt;
        ///
        ///&lt;/html&gt;.
        /// </summary>
        internal static string ResponsePage401 {
            get {
                return ResourceManager.GetString("ResponsePage401", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html lang=&quot;en&quot;&gt;
        ///
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width&quot;&gt;
        ///    &lt;style&gt;
        ///        body {
        ///            text-align: center;
        ///        }
        ///    &lt;/style&gt;
        ///    &lt;title&gt;403 Forbidden&lt;/title&gt;
        ///&lt;/head&gt;
        ///
        ///&lt;body&gt;
        ///    &lt;h1&gt;403 Forbidden&lt;/h1&gt;
        ///    &lt;p&gt;You don&apos;t have permission to access this resource&lt;/p&gt;
        ///    &lt;hr&gt;
        ///    &lt;p&gt;Simple HTTP server&lt;/p&gt;
        ///&lt;/body&gt;
        ///
        ///&lt;/html&gt;.
        /// </summary>
        internal static string ResponsePage403 {
            get {
                return ResourceManager.GetString("ResponsePage403", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html lang=&quot;en&quot;&gt;
        ///
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width&quot;&gt;
        ///    &lt;style&gt;
        ///        body {
        ///            text-align: center;
        ///        }
        ///    &lt;/style&gt;
        ///    &lt;title&gt;404 Not Found&lt;/title&gt;
        ///&lt;/head&gt;
        ///
        ///&lt;body&gt;
        ///    &lt;h1&gt;404 Not Found&lt;/h1&gt;
        ///    &lt;p&gt;The requested URL was not found on the server&lt;/p&gt;
        ///    &lt;hr&gt;
        ///    &lt;p&gt;Simple HTTP server&lt;/p&gt;
        ///&lt;/body&gt;
        ///
        ///&lt;/html&gt;.
        /// </summary>
        internal static string ResponsePage404 {
            get {
                return ResourceManager.GetString("ResponsePage404", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html lang=&quot;en&quot;&gt;
        ///
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width&quot;&gt;
        ///    &lt;style&gt;
        ///        body {
        ///            text-align: center;
        ///        }
        ///    &lt;/style&gt;
        ///    &lt;title&gt;405 Method Not Allowed&lt;/title&gt;
        ///&lt;/head&gt;
        ///
        ///&lt;body&gt;
        ///    &lt;h1&gt;405 Method Not Allowed&lt;/h1&gt;
        ///    &lt;p&gt;The target resource does not support this method&lt;/p&gt;
        ///    &lt;hr&gt;
        ///    &lt;p&gt;Simple HTTP server&lt;/p&gt;
        ///&lt;/body&gt;
        ///
        ///&lt;/html&gt;.
        /// </summary>
        internal static string ResponsePage405 {
            get {
                return ResourceManager.GetString("ResponsePage405", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html lang=&quot;en&quot;&gt;
        ///
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width&quot;&gt;
        ///    &lt;style&gt;
        ///        body {
        ///            text-align: center;
        ///        }
        ///    &lt;/style&gt;
        ///    &lt;title&gt;500 Internal Server Error&lt;/title&gt;
        ///&lt;/head&gt;
        ///
        ///&lt;body&gt;
        ///    &lt;h1&gt;500 Internal Server Error&lt;/h1&gt;
        ///    &lt;p&gt;The server encountered an unexpected condition that prevented it from fulfilling the request&lt;/p&gt;
        ///    &lt;hr&gt;
        ///    &lt;p&gt;Simple HTTP server&lt;/p&gt;
        ///&lt;/body&gt;
        ///
        ///&lt;/html&gt;.
        /// </summary>
        internal static string ResponsePage500 {
            get {
                return ResourceManager.GetString("ResponsePage500", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html lang=&quot;en&quot;&gt;
        ///
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width&quot;&gt;
        ///    &lt;style&gt;
        ///        body {
        ///            text-align: center;
        ///        }
        ///    &lt;/style&gt;
        ///    &lt;title&gt;501 Not Implemented&lt;/title&gt;
        ///&lt;/head&gt;
        ///
        ///&lt;body&gt;
        ///    &lt;h1&gt;501 Not Implemented&lt;/h1&gt;
        ///    &lt;p&gt;The server does not support the functionality required to fulfill the request&lt;/p&gt;
        ///    &lt;hr&gt;
        ///    &lt;p&gt;Simple HTTP server&lt;/p&gt;
        ///&lt;/body&gt;
        ///
        ///&lt;/html&gt;.
        /// </summary>
        internal static string ResponsePage501 {
            get {
                return ResourceManager.GetString("ResponsePage501", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html lang=&quot;en&quot;&gt;
        ///
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width&quot;&gt;
        ///    &lt;style&gt;
        ///        body {
        ///            text-align: center;
        ///        }
        ///    &lt;/style&gt;
        ///    &lt;title&gt;503 Service Unavailable&lt;/title&gt;
        ///&lt;/head&gt;
        ///
        ///&lt;body&gt;
        ///    &lt;h1&gt;503 Service Unavailable&lt;/h1&gt;
        ///    &lt;p&gt;The server is temporarily unable to service your request&lt;/p&gt;
        ///    &lt;hr&gt;
        ///    &lt;p&gt;Simple HTTP server&lt;/p&gt;
        ///&lt;/body&gt;
        ///
        ///&lt;/html&gt;.
        /// </summary>
        internal static string ResponsePage503 {
            get {
                return ResourceManager.GetString("ResponsePage503", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html lang=&quot;en&quot;&gt;
        ///
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width&quot;&gt;
        ///    &lt;style&gt;
        ///        body {
        ///            text-align: center;
        ///        }
        ///    &lt;/style&gt;
        ///    &lt;title&gt;505 HTTP Version Not Supported&lt;/title&gt;
        ///&lt;/head&gt;
        ///
        ///&lt;body&gt;
        ///    &lt;h1&gt;505 HTTP Version Not Supported&lt;/h1&gt;
        ///    &lt;p&gt;The HTTP version used in the request is not supported by the server&lt;/p&gt;
        ///    &lt;hr&gt;
        ///    &lt;p&gt;Simple HTTP server&lt;/p&gt;
        ///&lt;/body&gt;
        ///
        ///&lt;/html&gt;.
        /// </summary>
        internal static string ResponsePage505 {
            get {
                return ResourceManager.GetString("ResponsePage505", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 100 Continue
        ///101 Switching Protocols
        ///102 Processing
        ///200 OK
        ///201 Created
        ///202 Accepted
        ///203 Non-Authoritative Information
        ///204 No Content
        ///205 Reset Content
        ///206 Partial Content
        ///207 Multi-Status
        ///208 Already Reported
        ///226 IM Used
        ///300 Multiple Choices
        ///301 Moved Permanently
        ///302 Found
        ///303 See Other
        ///304 Not Modified
        ///305 Use Proxy
        ///307 Temporary Redirect
        ///308 Permanent Redirect
        ///400 Bad Request
        ///401 Unauthorized
        ///402 Payment Required
        ///403 Forbidden
        ///404 Not Found
        ///405 Method Not Allowed
        ///406 Not Accepta [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ResponseStatusMessages {
            get {
                return ResourceManager.GetString("ResponseStatusMessages", resourceCulture);
            }
        }
    }
}