﻿---
title: graph_path2term
---

# graph_path2term
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `graph_path2term`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `graph_path2term` (
 `graph_path_id` int(11) NOT NULL DEFAULT '0',
 `term_id` int(11) NOT NULL DEFAULT '0',
 `rank` int(11) NOT NULL DEFAULT '0',
 KEY `graph_path_id` (`graph_path_id`),
 KEY `term_id` (`term_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




