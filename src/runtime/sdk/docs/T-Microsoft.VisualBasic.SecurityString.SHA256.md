﻿---
title: SHA256
---

# SHA256
_namespace: [Microsoft.VisualBasic.SecurityString](N-Microsoft.VisualBasic.SecurityString.html)_

Derives a SHA256 key from a password using an extension of the PBKDF1 algorithm.



### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.SecurityString.SHA256.#ctor(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|password|-|
|saltValue|8 Bytes|


#### DecryptString
```csharp
Microsoft.VisualBasic.SecurityString.SHA256.DecryptString(System.String)
```
字符串的解密方法

|Parameter Name|Remarks|
|--------------|-------|
|cipherText|-|


#### EncryptData
```csharp
Microsoft.VisualBasic.SecurityString.SHA256.EncryptData(System.String)
```
Encrypt the plain text string.

|Parameter Name|Remarks|
|--------------|-------|
|plainText|-|


#### GetDynamicsCertification
```csharp
Microsoft.VisualBasic.SecurityString.SHA256.GetDynamicsCertification(System.Type)
```
双重动态数据签名

#### GetDynamicsCertification``1
```csharp
Microsoft.VisualBasic.SecurityString.SHA256.GetDynamicsCertification``1
```
双重动态数据签名


### Properties

#### CertificateSigned
The previous key of the sha256 encryption will be expired after the rebuild of this module,
 so that this method is not working on the statics data storage job.
 (在本模块进行重新编译之后，原有的密匙将会失效，故这个属性不适合于静态存储加密使用)
