﻿---
title: aaseq
---

# aaseq
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `aaseq`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `aaseq` (
 `locusId` int(10) unsigned DEFAULT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `sequence` longblob,
 UNIQUE KEY `Orf_Version` (`locusId`,`version`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




