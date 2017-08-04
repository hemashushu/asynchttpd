using System;
using System.Collections.Generic;
using System.Text;

namespace Doms.HttpService.HttpProtocol
{
    class HttpResponseStatusCode
    {
        public static string GetStatusName(System.Net.HttpStatusCode code)
        {
            //see RFC 2616 and RFC 2518

            string statusName = null;

            switch ((int)code)
            {
                #region Informational
                //The initial part of an incomplete request was accepted by the server. [HTTP] 
                case 100:
                    statusName = "Continue";
                    break;

                //The server will switch to a newer version, or real-time protocol, to continue. Used when an Upgrade header was included in the client request. [HTTP]
                case 101:
                    statusName = "Switching Protocols";
                    break;

                //MOVE Method, COPY Method
                //The status of ongoing processes, particularly those that use the Depth Header. The purpose of this code is to avoid having the client time out with an error. [WebDAV]
                case 102:
                    statusName = "Processing";
                    break;

                #endregion

                #region Successful
                //GET Method, HEAD Method, OPTIONS Method, LOCK Method, POST Method, DELETE Method, POLL Method, SUBSCRIBE Method, UNSUBSCRIBE Method
                //A typical successful response for these methods in cases where no new resource is created. [HTTP]
                //
                //TRACE Method
                //The trace was reflected back to the sender. [HTTP]
                //
                //PROPFIND Method, PROPPATCH Method
                //A possible successful response if all props were returned successfully or set. Not used in Microsoft implementations. See Code 207. [WebDAV]
                case 200:
                    statusName = "OK";
                    break;

                //POST Method, PUT Method
                //The resource was created successfully. [HTTP] If the action cannot be completed immediately, a 202 must be returned instead. [HTTP/1.1]
                //
                //MKCOL Method, MOVE Method
                //A collection was created successfully or a resource was moved successfully. [WebDAV]
                //
                //LOCK Method
                //A lock was done on a null resource and the LOCK Method created the resource.
                case 201:
                    statusName = "Created";
                    break;

                //DELETE Method, PUT Method, POST Method
                //The resource will be created or deleted, but this has not happened yet. [HTTP]
                case 202:
                    statusName = "Accepted";
                    break;

                //The metadata in the header did not come from the originating server. Used only when the response would otherwise be 200 OK. [HTTP] Use of this response code is not required. [HTTP/1.1]
                case 203:
                    statusName = "Non Authoritative Information";
                    break;

                //POST Method, PUT Method, POLL Method
                //A standard success status code when no resource was created [HTTP]. The server has fulfilled the request but does not need to return an entity body, and might return updated metadata. [HTTP 1.1]
                //With PUT, the 204 response allows the server to send back an updated etag and other entity information about the resource that has been affected by the PUT operation. This allows the client to do the next PUT using the If-Match precondition to ensure that edits are not lost.
                //
                //DELETE Method, UNLOCK Method
                //A standard success response. [WebDAV]
                //
                //COPY Method, MOVE Method
                //The source resource was copied successfully or moved to a pre-existing destination resource. [HTTP]
                case 204:
                    statusName = "No Content";
                    break;

                //The server has fulfilled the request and the client should reset the document view. Must not include an entity. [HTTP]
                case 205:
                    statusName = "Reset Content";
                    break;

                //GET Method
                //Some of the resource was returned; the Range Header indicates how much. [HTTP]
                case 206:
                    statusName = "Partial Content";
                    break;

                //PROPFIND Method, PROPPATCH Method, SEARCH Method, MKCOL Method, POLL Method, SUBSCRIBE Method, UNSUBSCRIBE Method
                //A typical successful response. [WebDAV]
                //
                //COPY Method, MOVE Method
                //An error in executing the COPY Method occurred with a resource other than the request Uniform Resource Identifier (URI). 424 and 201 responses should not be values in 207 Multi-Status responses to the COPY Method. [WebDAV]
                //
                //LOCK Method
                //The LOCK method was used to perform a multi-resource lock request. [WebDAV]
                case 207:
                    statusName = "Multi Status";
                    break;

                //PUT Method, MOVE Method, COPY Method
                //In a WebDAV replication context, there is mismatch between the content and/or properties on the client resource and the content and/or properties on the sever resource, reflected by the resourcetag. See HTTP and WebDAV Methods for Replication.
                case 210:
                    statusName = "Content Different";
                    break;

                #endregion

                #region Redirection
                //GET Method, HEAD Method
                //The requested resource corresponds to any one of a set of representations, each with its own specific location. Agent-driven negotiation information (section 12) is being provided so that the user (or user agent) can select a preferred representation and redirect its request to that location. [HTTP]
                case 300:
                    statusName = "Multiple Choices";
                    break;

                //GET Method, HEAD Method
                //A new permanent Uniform Resource Identifier (URI) has been assigned to the resource. [HTTP]
                //
                //PUT Method
                //The server wants to apply the PUT to a different URI than the one in the method.
                case 301:
                    statusName = "Moved Permanently";
                    break;


                //GET Method, HEAD Method
                //A new temporary URI has been assigned to the resource. [HTTP]
                case 302:
                    statusName = "Moved Temporarily"; //also call "Found"
                    break;


                //POST Method
                //This status code is typically used to respond to a POST, because a response to a POST cannot be stored in the cache, but the response to a GET can be. [HTTP]
                case 303:
                    statusName = "See Other";
                    break;

                //If the client has performed a conditional GET request and access is allowed, but the document has not been modified, the server should respond with this status code. The response must not contain a message-body. [HTTP]
                case 304:
                    statusName = "Not Modified";
                    break;

                //The requested resource must be accessed through the proxy given by the Location field. [HTTP/1.1]
                case 305:
                    statusName = "UseProxy";
                    break;

                //The 306 status code was used in a previous version of the specification and is no longer used. Also, the code is reserved. [HTTP/1.1]
                case 306:
                    statusName = "Unused";
                    break;

                //GET Method, HEAD Method
                //The requested resource resides temporarily under a different URI.
                case 307:
                    statusName = "Temporary Redirect";
                    break;

                #endregion

                #region Client error
                //Any
                //Indicates malformed syntax (for example, an empty namespace name in the XML body). [HTTP]
                //
                //SEARCH Method
                //The SEARCH Method request did not contain an XML body, or the search scope was invalid, or the query could not be executed.
                case 400:
                    statusName = "Bad Request";
                    break;

                //Any
                //The resource requires authorization or authorization was refused. [HTTP]
                //
                //PROPPATCH Method
                //If a PROPPATCH Method is executed on two properties, one of which requires authentication to change and the other does not, the entire request will be rejected with a 401.
                case 401:
                    statusName = "Unauthorized";
                    break;

                case 402:
                    statusName = "Payment Required";
                    break;

                //Any
                //The client submitted a request that the server will not perform.
                //
                //MKCOL Method, LOCK Method, PROPPATCH Method
                //The client does not have permissions to make a collection, lock the resource, or set a property. [WebDAV] Check-out attempt: this will be returned if the resource is not versioned or if the resource is a collection.
                case 403:
                    statusName = "Forbidden";
                    break;

                //The resource named was not found. [HTTP]
                case 404:
                    statusName = "Not Found";
                    break;

                //Any
                //The method was not allowed for the resource that was named. [HTTP] The response must include an Allow header containing a list of valid methods for the requested resource. [HTTP/1.1]
                //
                //MKCOL Method
                //This might occur if the resource already exists and thus cannot be created. [WebDAV]
                case 405:
                    statusName = "Method Not Allowed";
                    break;

                //Any
                //The resource identified by the request is capable only of generating response entities that have content characteristics that are not acceptable according to the Accept headers sent in the request. The response should include an entity containing a list of available entity characteristics and location(s) from which the user or user agent can choose the most appropriate one. [HTTP]
                //
                //HEAD Method
                //The response need not include an entity as indicated above.
                case 406:
                    statusName = "Not Acceptable";
                    break;


                //This code is similar to 401 (Unauthorized), but it indicates that the client must first authenticate itself with the proxy. [HTTP/1.1]
                case 407:
                    statusName = "Proxy Authentication Required";
                    break;

                //The client did not produce a request within the time that the server was set to wait. [HTTP/1.1]
                case 408:
                    statusName = "RequestTimeout";
                    break;

                //MKCOL Method
                //A collection cannot be created until intermediate collections have been created. [WebDAV]
                //
                //PROPPATCH Method
                //The client has provided a value, the semantics of which are not appropriate for the property. Example: trying to set a read-only property. [WebDAV]
                //
                //PUT Method
                //Cannot PUT a resource if all ancestors do not already exist. [WebDAV]
                //If versioning is being used and the entity being PUT includes changes to a resource that conflict with those made by an earlier (third-party) request, the server might use the 409 response code to indicate that it cannot complete the request. [HTTP/1.1]
                case 409:
                    statusName = "Conflict";
                    break;

                //GET Method
                //The requested resource is no longer available on the server and no forwarding address is known. This condition is considered permanent. [HTTP/1.1]
                case 410:
                    statusName = "Gone";
                    break;

                //The server refuses to accept the request without a defined Content-Length header. [HTTP/1.1]
                case 411:
                    statusName = "Length Required";
                    break;

                //Any
                //The precondition given in one or more of the Request header fields evaluated to false when it was tested on the server. [HTTP/1.1]
                //
                //COPY Method, MOVE Method
                //The server was unable to maintain the availability of the properties listed in the property behavior XML element, or the Overwrite Header is F and the state of the destination resource is not null. [WebDAV]
                //
                //LOCK Method
                //The lock token could not be enforced or the server could not satisfy the request in the lockinfo XML element.
                //
                //PUT Method, COPY Method
                //In a WebDAV replication context, either the If: (<resourcetag>) or If: (<repl-uid>) request header in the PUT request evaluated to FALSE.
                case 412:
                    statusName = "Precondition Failed";
                    break;

                //The server refuses to process the request because it cannot process a request entity this large. [HTTP/1.1]
                case 413:
                    statusName = "Request Entity Too Large";
                    break;

                //The server cannot service the request because the Request-URI is longer than what the server can interpret. [HTTP/1.1]
                case 414:
                    statusName = "Request Uri Too Long";
                    break;

                //The server cannot service the request because the entity of the request is in a format that is not supported by the requested resource for the requested method.
                //MKCOL Method
                //The server does not support the request type of the body. [WebDAV]
                case 415:
                    statusName = "Unsupported Media Type";
                    break;

                //A server should return a response with this status code if a request included a Range Header field (section 14.35), if none of the range-specifier values in this field overlap with the current extent of the selected resource, and if the request did not include an If-Range request-header field. [HTTP/1.1]
                case 416:
                    statusName = "Requested Range Not Satisfiable";
                    break;

                //The expectation given in an Expect request-header field (see section 14.20) could not be met by this server or, if the server is a proxy, the server has unambiguous evidence that the next-hop server could not meet the request. [HTTP/1.1]
                case 417:
                    statusName = "Expectation Failed";
                    break;

                //MKCOL Method
                //The server understands the content type of the request entity, but is unable to process the instructions that it contains. [WebDAV] For example, the MKCOL Method has a request body that is not understood or is incomplete, even if in XML, which is generally accepted in this situation.
                //
                //SEARCH Method
                //There is a valid XML body in a SEARCH Method, but an unsupported or unimplemented query operator.
                //
                //LOCK Method
                //A non-zero depth was specified for a check-out lock, or the Depth Header was omitted.
                case 422:
                    statusName = "Unprocessable Entity";
                    break;


                //PROPPATCH Method, DELETE Method, MOVE Method
                //Properties cannot be set, deleted, or moved, and a locked resource cannot be locked with the lock token. [WebDAV]
                //
                //LOCK Method
                //The resource is already locked. The lock token is needed to refresh the lock. [WebDAV]
                //
                //COPY Method
                //The destination resource was locked. [WebDAV]
                case 423:
                    statusName = "Locked";
                    break;


                //PROPPATCH Method
                //The method was not executed on a resource because some part of the method's execution failed and the whole method was aborted. [WebDAV]
                //
                //UNLOCK Method
                //One of the methods in the transaction failed. Therefore the entire transaction failed.
                case 424:
                    statusName = "Method Failure";
                    break;


                //SEARCH Method
                //The query produced more results than the server can transmit. Partial results have been transmitted.
                case 425:
                    statusName = "Insufficient Spaceon Resource";
                    break;

                //SEARCH Method
                //The collblob submitted was invalid or outdated.
                case 475:
                    statusName = "Invalidcollblob";
                    break;
                #endregion

                #region Server error
                //The server encountered an unexpected condition that prevented it from fulfilling the request. [HTTP/1.1]
                case 500:
                    statusName = "Internal Server Error";
                    break;

                //Any
                //The server does not support the functionality that is required to fulfill the request. This is the appropriate response when the server does not recognize the request method and is not capable of supporting it for any resource. [HTTP/1.1]. Contrast with Status Code 405.
                //
                //PUT Method
                //Error when a server receives a Content-* header it does not understand in a PUT operation.
                case 501:
                    statusName = "Not Implemented";
                    break;

                //The server, while acting as a gateway or proxy, received an invalid response from the upstream server that it accessed in attempting to fulfill the request. [HTTP/1.1]
                //COPY Method, MOVE Method
                //The destination could be on another server and the destination server will not accept the resource. [WebDAV]
                case 502:
                    statusName = "Bad Gateway";
                    break;

                //The server is currently unable to handle the request due to temporary overloading or to maintenance of the server. The implication is that this is a temporary condition. [HTTP/1.1]
                case 503:
                    statusName = "Service Unavailable";
                    break;

                //The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server specified by the Uniform Resource Identifier (URI) (for example, HTTP, FTP, LDAP) or some other auxiliary server (for example, DNS) that it needed to access to complete the request. [HTTP/1.1]
                case 504:
                    statusName = "Gateway Timeout";
                    break;

                //The server does not support the HTTP protocol version that was used in the request message. [HTTP/1.1]
                case 505:
                    statusName = "Http Version Not Supported";
                    break;

                //PROPPATCH Method, MKCOL Method
                //The resource does not have enough space to set the properties or make the collection. [WebDAV]
                //
                //COPY Method
                //The destination resource does not have sufficient space. [WebDAV]
                //
                //SEARCH Method
                //The query produced more results than the server is able to transmit. Partial results have been transmitted. The server must send a body that matches the body for Code 207, except that there may exist resources that match the search criteria for which no corresponding response exists in the reply.
                case 507:
                    statusName = "InsufficientStorage";
                    break;

                #endregion

                default:
                    throw new InvalidOperationException("No this http status code");
            }

            return statusName;
        }

    }//end class
}
