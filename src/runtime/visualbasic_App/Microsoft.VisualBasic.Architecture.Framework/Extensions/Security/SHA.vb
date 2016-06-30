﻿Imports System.Security
Imports System.Security.Cryptography
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports System.Text
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace SecurityString

    ''' <summary>
    ''' Derives a SHA256 key from a password using an extension of the PBKDF1 algorithm.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SHA256 : Inherits SecurityStringModel

        Const hashAlgorithm As String = "SHA1"
        Const keySize As Integer = 256
        Const passwordIterations As Integer = 2

        ' Convert strings defining encryption key characteristics into byte
        ' arrays. Let us assume that strings only contain ASCII codes.
        ' If strings include Unicode characters, use Unicode, UTF7, or UTF8
        ' encoding.

        Dim initVectorBytes As Byte()
        Dim saltValueBytes As Byte()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="password"></param>
        ''' <param name="saltValue">8 Bytes</param>
        Sub New(password As String, saltValue As String)
            Const initVector = "@1B2c3D4e5F6g7H8"

            Me.strPassphrase = password
            Me.initVectorBytes = Encoding.ASCII.GetBytes(initVector)
            Me.saltValueBytes = Encoding.ASCII.GetBytes(saltValue)
        End Sub

        Public Overrides Function ToString() As String
            Return MyBase.ToString() & $"//  {NameOf(saltValueBytes)}:={String.Join(", ", saltValueBytes.ToArray(Of String)(Function(b) CStr(b)))}"
        End Function

        Public ReadOnly Property Passphrase As String
            Get
                Return Me.strPassphrase
            End Get
        End Property

        Public Overloads Overrides Function Decrypt(cipherTextBytes As Byte()) As Byte()

            ' First, we must create a password, from which the key will be
            ' derived. This password will be generated from the specified
            ' passphrase and salt value. The password will be created using
            ' the specified hash algorithm. Password creation can be done in
            ' several iterations.

            Dim password As New PasswordDeriveBytes(strPassphrase, saltValueBytes, hashAlgorithm, passwordIterations)

            ' Use the password to generate pseudo-random bytes for the encryption
            ' key. Specify the size of the key in bytes (instead of bits).
#Disable Warning
            Dim keyBytes As Byte() = password.GetBytes(keySize \ 8)
#Enable Warning
            ' Create uninitialized Rijndael encryption object.

            Dim symmetricKey As New RijndaelManaged()

            ' It is reasonable to set encryption mode to Cipher Block Chaining
            ' (CBC). Use default options for other symmetric key parameters.

            symmetricKey.Mode = CipherMode.CBC

            ' Generate decryptor from the existing key bytes and initialization
            ' vector. Key size will be defined based on the number of the key
            ' bytes.

            Dim decryptor As ICryptoTransform = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes)

            ' Define memory stream which will be used to hold encrypted data.

            Dim memoryStream As New MemoryStream(cipherTextBytes)

            ' Define cryptographic stream (always use Read mode for encryption).

            Dim cryptoStream As New CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)

            ' Since at this point we don't know what the size of decrypted data
            ' will be, allocate the buffer long enough to hold ciphertext;
            ' plaintext is never longer than ciphertext.

            Dim plainTextBytes As Byte() = New Byte(cipherTextBytes.Length - 1) {}

            ' Start decrypting.

            Dim decryptedByteCount As Integer = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)

            ' Close both streams.

            memoryStream.Close()
            cryptoStream.Close()

            ' Convert decrypted data into a string.
            ' Let us assume that the original plaintext string was UTF8-encoded.

            Return plainTextBytes
        End Function

        ''' <summary>
        ''' 字符串的解密方法
        ''' </summary>
        ''' <param name="cipherText"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DecryptString(cipherText As String) As String

            ' Convert our ciphertext into a byte array.

            Dim cipherTextBytes As Byte() = Convert.FromBase64String(cipherText)

            ' First, we must create a password, from which the key will be
            ' derived. This password will be generated from the specified
            ' passphrase and salt value. The password will be created using
            ' the specified hash algorithm. Password creation can be done in
            ' several iterations.

            Dim password As New PasswordDeriveBytes(strPassphrase, saltValueBytes, hashAlgorithm, passwordIterations)

            ' Use the password to generate pseudo-random bytes for the encryption
            ' key. Specify the size of the key in bytes (instead of bits).
#Disable Warning
            Dim keyBytes As Byte() = password.GetBytes(keySize \ 8)
#Enable Warning
            ' Create uninitialized Rijndael encryption object.

            Dim symmetricKey As New RijndaelManaged()

            ' It is reasonable to set encryption mode to Cipher Block Chaining
            ' (CBC). Use default options for other symmetric key parameters.

            symmetricKey.Mode = CipherMode.CBC

            ' Generate decryptor from the existing key bytes and initialization
            ' vector. Key size will be defined based on the number of the key
            ' bytes.

            Dim decryptor As ICryptoTransform = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes)

            ' Define memory stream which will be used to hold encrypted data.

            Dim memoryStream As New MemoryStream(cipherTextBytes)

            ' Define cryptographic stream (always use Read mode for encryption).

            Dim cryptoStream As New CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)

            ' Since at this point we don't know what the size of decrypted data
            ' will be, allocate the buffer long enough to hold ciphertext;
            ' plaintext is never longer than ciphertext.

            Dim plainTextBytes As Byte() = New Byte(cipherTextBytes.Length - 1) {}

            ' Start decrypting.

            Dim decryptedByteCount As Integer = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)

            ' Close both streams.

            memoryStream.Close()
            cryptoStream.Close()

            ' Convert decrypted data into a string.
            ' Let us assume that the original plaintext string was UTF8-encoded.
            Dim plainText As String = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount)
            Return plainText
        End Function

        Public Overloads Overrides Function Encrypt(plainTextBytes() As Byte) As Byte()
            Dim password As New PasswordDeriveBytes(strPassphrase, saltValueBytes, hashAlgorithm, passwordIterations)
#Disable Warning
            Dim keyBytes As Byte() = password.GetBytes(keySize \ 8)
#Enable Warning
            Dim symmetricKey As New RijndaelManaged()

            symmetricKey.Mode = CipherMode.CBC

            Dim encryptor As ICryptoTransform = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)
            Dim memoryStream As New MemoryStream()
            Dim cryptoStream As New CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)

            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length)
            cryptoStream.FlushFinalBlock()

            Dim cipherTextBytes As Byte() = memoryStream.ToArray()

            memoryStream.Close()
            cryptoStream.Close()

            Return cipherTextBytes
        End Function

        Public Function Serialization(data As Byte()) As String
            data = Encrypt(data)
            Return System.Text.Encoding.Unicode.GetString(data)
        End Function

        Public Function Deserialization(data As String) As Byte()
            Dim Buffer As Byte() = System.Text.Encoding.Unicode.GetBytes(data)
            Return Decrypt(Buffer)
        End Function

        ''' <summary>
        ''' Encrypt the plain text string.
        ''' </summary>
        ''' <param name="plainText"></param>
        ''' <returns></returns>
        Public Overrides Function EncryptData(plainText As String) As String

            Dim plainTextBytes As Byte() = Encoding.UTF8.GetBytes(plainText)
            Dim password As New PasswordDeriveBytes(strPassphrase, saltValueBytes, hashAlgorithm, passwordIterations)
#Disable Warning
            Dim keyBytes As Byte() = password.GetBytes(keySize \ 8)
#Enable Warning
            Dim symmetricKey As New RijndaelManaged()

            symmetricKey.Mode = CipherMode.CBC

            Dim encryptor As ICryptoTransform = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)
            Dim memoryStream As New MemoryStream()
            Dim cryptoStream As New CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)

            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length)
            cryptoStream.FlushFinalBlock()

            Dim cipherTextBytes As Byte() = memoryStream.ToArray()

            memoryStream.Close()
            cryptoStream.Close()

            Dim cipherText As String = Convert.ToBase64String(cipherTextBytes)

            Return cipherText
        End Function

        ''' <summary>
        ''' The previous key of the sha256 encryption will be expired after the rebuild of this module,
        ''' so that this method is not working on the statics data storage job.
        ''' (在本模块进行重新编译之后，原有的密匙将会失效，故这个属性不适合于静态存储加密使用)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property CertificateSigned As SHA256 =
            New SHA256(password:=Microsoft.VisualBasic.SecurityString.GetFileHashString(GetType(SHA256).Assembly.Location), saltValue:="ATCGCGAT")

        ''' <summary>
        ''' 双重动态数据签名
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDynamicsCertification(Of T)() As SHA256
            Return GetDynamicsCertification(GetType(T))
        End Function

        ''' <summary>
        ''' 双重动态数据签名
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDynamicsCertification(TypeInfo As Type) As SHA256
            Dim Password As String = Microsoft.VisualBasic.SecurityString.GetFileHashString(TypeInfo.Assembly.Location)
            Dim saltValue As String = Microsoft.VisualBasic.SecurityString.GetFileHashString(GetType(SHA256).Assembly.Location)
            saltValue = Mid(saltValue, 3, 8)
            Return New SHA256(Password, saltValue)
        End Function
    End Class
End Namespace