﻿#Region "Microsoft.VisualBasic::1a0682958f4bf066e729e6b3c54eb630, ..\VisualBasic_AppFramework\Microsoft.VisualBasic.NETProtocol\AppServer\PushAPI\UserAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Net
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection

Namespace PushAPI

    ''' <summary>
    ''' 对User client开放的协议接口，也就是用户的客户端是通过这个模块来发送消息或者读取自己的消息
    ''' </summary>
    <Protocol(GetType(Protocols.UserAPI.Protocols))>
    Public Class UserAPI : Inherits APIBase

        ''' <summary>
        ''' 用户编号转换为程序之中的唯一标识符
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property UserHash As New Dictionary(Of String, Long)
        ''' <summary>
        ''' 反向查找<see cref="UserHash"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property HashUser As New Dictionary(Of Long, String)

        Sub New(push As PushServer)
            Call MyBase.New(push)
            __protocols = New ProtocolHandler(Me)
        End Sub

        Public Overrides Function Handler(CA As Long, request As RequestStream, remote As System.Net.IPEndPoint) As RequestStream
            Return __protocols.HandleRequest(CA, request, remote)
        End Function

        ''' <summary>
        ''' 第一步，初始化哈希表
        ''' </summary>
        ''' <param name="CA"></param>
        ''' <param name="request">user_id</param>
        ''' <param name="remote"></param>
        ''' <returns></returns>
        ''' 
        <Protocol(Protocols.UserAPI.Protocols.InitUser)>
        Private Function __userInitPOST(CA As Long, request As RequestStream, remote As System.Net.IPEndPoint) As RequestStream
            Dim sId As String = request.GetUTF8String
            Dim uid As Long = Protocols.UserAPI.Uid(sId)

            If UserHash.ContainsKey(sId) Then
            Else
                Call UserHash.Add(sId, uid)
                Call HashUser.Add(uid, sId)
            End If

            Dim post As New Protocols.InitPOSTBack With {
                .uid = uid,
                .Portal = New IPEndPoint("", Me.PushServer.UserSocket.LocalPort) ' 在客户端已处理
            }
            Return RequestStream.CreatePackage(post)
        End Function

        ''' <summary>
        ''' 判断这个用户编号是否可用有效？
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function IsValid(id As Protocols.UserId) As Boolean
            If Not UserHash.ContainsKey(id.sId) Then
                Return False
            Else
                Return UserHash(id.sId) = id.uid
            End If
        End Function

        ''' <summary>
        ''' 用户客户端尝试得到消息数据
        ''' </summary>
        ''' <param name="CA"></param>
        ''' <param name="request"></param>
        ''' <param name="remote"></param>
        ''' <returns></returns>
        <Protocol(Protocols.UserAPI.Protocols.GetData)>
        Private Function __getData(CA As Long, request As RequestStream, remote As System.Net.IPEndPoint) As RequestStream
            Dim id = request.LoadObject(Of Protocols.UserId)(AddressOf Serialization.LoadObject)
            If Not IsValid(id) Then
                Return NetResponse.RFC_FORBIDDEN
            End If
            Dim msg As RequestStream = Me.PushServer.GetMsg(id.uid)
            Return msg
        End Function
    End Class
End Namespace
