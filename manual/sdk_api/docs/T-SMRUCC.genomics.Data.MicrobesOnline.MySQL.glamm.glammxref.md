﻿---
title: glammxref
---

# glammxref
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm.html)_

--
 
 DROP TABLE IF EXISTS `glammxref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `glammxref` (
 `guid` bigint(10) unsigned NOT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `priority` tinyint(3) unsigned DEFAULT NULL,
 `created` date DEFAULT NULL,
 `fromGuid` bigint(10) unsigned NOT NULL,
 `toXrefId` varchar(50) NOT NULL DEFAULT '',
 `xrefDbName` varchar(50) NOT NULL DEFAULT '',
 `xrefUrl` text,
 PRIMARY KEY (`guid`),
 KEY `fromGuid` (`fromGuid`),
 KEY `toXrefId` (`toXrefId`),
 KEY `xrefDbName` (`xrefDbName`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




