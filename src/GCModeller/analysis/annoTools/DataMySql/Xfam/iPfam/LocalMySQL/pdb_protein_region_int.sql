-- MySQL dump 10.11
--
-- Host: localhost    Database: ipfam_1_0
-- ------------------------------------------------------
-- Server version	5.5.14-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `pdb_protein_region_int`
--

DROP TABLE IF EXISTS `pdb_protein_region_int`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `pdb_protein_region_int` (
  `auto_reg_int` bigint(20) NOT NULL AUTO_INCREMENT,
  `pdb_id` varchar(4) NOT NULL,
  `region_id_A` int(10) unsigned NOT NULL,
  `region_id_B` int(10) unsigned NOT NULL,
  `intrachain` tinyint(1) NOT NULL,
  `quality_control` int(10) unsigned NOT NULL,
  PRIMARY KEY (`auto_reg_int`),
  KEY `ddi_quality_control_Idx` (`quality_control`),
  KEY `fk_pdb_protein_region_int_pdb_protein_region1_idx` (`region_id_A`),
  KEY `fk_pdb_protein_region_int_pdb_protein_region2_idx` (`region_id_B`),
  CONSTRAINT `fk_pdb_protein_region_int_pdb_protein_region1` FOREIGN KEY (`region_id_A`) REFERENCES `pdb_protein_region` (`region_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_pdb_protein_region_int_pdb_protein_region2` FOREIGN KEY (`region_id_B`) REFERENCES `pdb_protein_region` (`region_id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=346475 DEFAULT CHARSET=latin1 COMMENT='domain domain interactions';
SET character_set_client = @saved_cs_client;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2013-09-08 14:01:19
