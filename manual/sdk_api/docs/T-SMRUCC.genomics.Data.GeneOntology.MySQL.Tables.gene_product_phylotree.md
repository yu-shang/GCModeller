﻿---
title: gene_product_phylotree
---

# gene_product_phylotree
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_phylotree`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_phylotree` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `gene_product_id` int(11) NOT NULL,
 `phylotree_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 KEY `gene_product_id` (`gene_product_id`),
 KEY `phylotree_id` (`phylotree_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




