-- MySQL dump 10.14  Distrib 5.5.60-MariaDB, for Linux (x86_64)
--
-- Host: localhost    Database: oerp
-- ------------------------------------------------------
-- Server version	5.5.60-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `account_2`
--

DROP TABLE IF EXISTS `account_2`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `account_2` (
  `account_id` int(11) NOT NULL AUTO_INCREMENT,
  `account_name` varchar(40) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `parent_id` int(1) NOT NULL,
  `status` int(11) NOT NULL,
  PRIMARY KEY (`account_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `account_2`
--

LOCK TABLES `account_2` WRITE;
/*!40000 ALTER TABLE `account_2` DISABLE KEYS */;
/*!40000 ALTER TABLE `account_2` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bank_add`
--

DROP TABLE IF EXISTS `bank_add`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bank_add` (
  `bank_id` varchar(255) NOT NULL,
  `bank_name` varchar(255) NOT NULL,
  `ac_name` varchar(250) DEFAULT NULL,
  `ac_number` varchar(250) DEFAULT NULL,
  `branch` varchar(250) DEFAULT NULL,
  `signature_pic` varchar(250) DEFAULT NULL,
  `status` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bank_add`
--

LOCK TABLES `bank_add` WRITE;
/*!40000 ALTER TABLE `bank_add` DISABLE KEYS */;
/*!40000 ALTER TABLE `bank_add` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bank_summary`
--

DROP TABLE IF EXISTS `bank_summary`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bank_summary` (
  `bank_id` varchar(250) DEFAULT NULL,
  `description` text,
  `deposite_id` varchar(250) DEFAULT NULL,
  `date` varchar(250) DEFAULT NULL,
  `ac_type` varchar(50) DEFAULT NULL,
  `dr` float DEFAULT NULL,
  `cr` float DEFAULT NULL,
  `ammount` float DEFAULT NULL,
  `status` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bank_summary`
--

LOCK TABLES `bank_summary` WRITE;
/*!40000 ALTER TABLE `bank_summary` DISABLE KEYS */;
/*!40000 ALTER TABLE `bank_summary` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cheque_manger`
--

DROP TABLE IF EXISTS `cheque_manger`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cheque_manger` (
  `cheque_id` varchar(100) NOT NULL,
  `transection_id` varchar(100) NOT NULL,
  `customer_id` varchar(100) NOT NULL,
  `bank_id` varchar(100) NOT NULL,
  `cheque_no` varchar(100) NOT NULL,
  `date` varchar(250) DEFAULT NULL,
  `transection_type` varchar(100) NOT NULL,
  `cheque_status` int(10) NOT NULL,
  `amount` float NOT NULL,
  `status` int(2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cheque_manger`
--

LOCK TABLES `cheque_manger` WRITE;
/*!40000 ALTER TABLE `cheque_manger` DISABLE KEYS */;
/*!40000 ALTER TABLE `cheque_manger` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `company_information`
--

DROP TABLE IF EXISTS `company_information`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `company_information` (
  `company_id` varchar(250) NOT NULL,
  `company_name` varchar(50) NOT NULL,
  `email` varchar(50) NOT NULL,
  `address` text NOT NULL,
  `mobile` varchar(15) NOT NULL,
  `website` varchar(50) NOT NULL,
  `status` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `company_information`
--

LOCK TABLES `company_information` WRITE;
/*!40000 ALTER TABLE `company_information` DISABLE KEYS */;
INSERT INTO `company_information` VALUES ('1','Glanzend Pvt. LTD','info@glanzand.com','Glanzend private limited House no 748, Block N Makkah chowk Sabzazar Lahore','(+92) 423750867','https://www.glanzend.com.pk/',1);
/*!40000 ALTER TABLE `company_information` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_information`
--

DROP TABLE IF EXISTS `customer_information`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `customer_information` (
  `customer_id` varchar(250) NOT NULL,
  `customer_name` varchar(255) DEFAULT NULL,
  `customer_address` varchar(255) NOT NULL,
  `customer_mobile` varchar(100) NOT NULL,
  `customer_email` varchar(100) NOT NULL,
  `status` int(2) NOT NULL COMMENT '1=paid,2=credit',
  `create_date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_information`
--

LOCK TABLES `customer_information` WRITE;
/*!40000 ALTER TABLE `customer_information` DISABLE KEYS */;
INSERT INTO `customer_information` VALUES ('6YT5Q3RKS5BDAC8','ALI LACE STORE','Old Post Office Near Wapda Complaint Office Baghbanpura, Lahore, Pakistan','+923214520646','',2,'2020-02-17 09:30:02'),('ZNNAVOHDQ22EE24','HAFIZ COSMETICS','OPP. Ravi Market Main Bazaar, Ichra Lahore, Pakistan  ','+924237426292','',2,'2020-02-14 13:29:18'),('96T47PQ9VYKPJRD','CASH & CARRY','CANTT, LAHORE PAKISTAN','','',2,'2020-02-14 13:30:40');
/*!40000 ALTER TABLE `customer_information` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_ledger`
--

DROP TABLE IF EXISTS `customer_ledger`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `customer_ledger` (
  `id` int(20) NOT NULL AUTO_INCREMENT,
  `transaction_id` varchar(100) NOT NULL,
  `customer_id` varchar(100) NOT NULL,
  `invoice_no` varchar(100) DEFAULT NULL,
  `receipt_no` varchar(50) DEFAULT NULL,
  `amount` float DEFAULT NULL,
  `description` varchar(255) NOT NULL,
  `payment_type` varchar(255) NOT NULL,
  `cheque_no` varchar(255) NOT NULL,
  `date` varchar(250) DEFAULT NULL,
  `receipt_from` varchar(50) DEFAULT NULL,
  `status` int(2) NOT NULL,
  `d_c` varchar(10) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=50 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_ledger`
--

LOCK TABLES `customer_ledger` WRITE;
/*!40000 ALTER TABLE `customer_ledger` DISABLE KEYS */;
INSERT INTO `customer_ledger` VALUES (47,'H1LGREM2QV','6YT5Q3RKS5BDAC8','NA',NULL,300000,'Previous adjustment with software','NA','NA','2020-02-17',NULL,1,'d','2020-02-17 09:30:02'),(33,'LRQKK4DGY1','ZNNAVOHDQ22EE24','NA',NULL,300000,'Previous adjustment with software','NA','NA','2020-02-14',NULL,1,'d','2020-02-14 13:29:18'),(34,'Y48Z8IJYAV','96T47PQ9VYKPJRD','NA',NULL,100000,'Previous adjustment with software','NA','NA','2020-02-14',NULL,1,'d','2020-02-14 13:30:40');
/*!40000 ALTER TABLE `customer_ledger` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary table structure for view `customer_transection_summary`
--

DROP TABLE IF EXISTS `customer_transection_summary`;
/*!50001 DROP VIEW IF EXISTS `customer_transection_summary`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `customer_transection_summary` (
  `customer_name` tinyint NOT NULL,
  `customer_id` tinyint NOT NULL,
  `type` tinyint NOT NULL,
  `amount` tinyint NOT NULL
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `daily_banking_add`
--

DROP TABLE IF EXISTS `daily_banking_add`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `daily_banking_add` (
  `banking_id` varchar(255) NOT NULL,
  `date` datetime NOT NULL,
  `bank_id` varchar(100) NOT NULL,
  `deposit_type` varchar(255) NOT NULL,
  `transaction_type` varchar(255) NOT NULL,
  `description` text NOT NULL,
  `amount` int(11) NOT NULL,
  `status` int(2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `daily_banking_add`
--

LOCK TABLES `daily_banking_add` WRITE;
/*!40000 ALTER TABLE `daily_banking_add` DISABLE KEYS */;
/*!40000 ALTER TABLE `daily_banking_add` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `daily_closing`
--

DROP TABLE IF EXISTS `daily_closing`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `daily_closing` (
  `closing_id` varchar(255) NOT NULL,
  `last_day_closing` float NOT NULL,
  `cash_in` float NOT NULL,
  `cash_out` float NOT NULL,
  `date` varchar(250) NOT NULL,
  `amount` float NOT NULL,
  `adjustment` float DEFAULT NULL,
  `status` int(2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `daily_closing`
--

LOCK TABLES `daily_closing` WRITE;
/*!40000 ALTER TABLE `daily_closing` DISABLE KEYS */;
INSERT INTO `daily_closing` VALUES ('RQbZ8Z9bfCp8GPR',0,10000,20000,'2020-01-24',0,NULL,1);
/*!40000 ALTER TABLE `daily_closing` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `invoice`
--

DROP TABLE IF EXISTS `invoice`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `invoice` (
  `invoice_id` varchar(100) NOT NULL,
  `customer_id` varchar(100) NOT NULL,
  `date` varchar(50) DEFAULT NULL,
  `total_amount` float NOT NULL,
  `invoice` varchar(255) NOT NULL,
  `invoice_discount` float DEFAULT NULL COMMENT 'invoice discount',
  `total_discount` float DEFAULT NULL COMMENT 'total invoice discount',
  `total_tax` float DEFAULT NULL,
  `invoice_details` text NOT NULL,
  `status` int(2) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `invoice`
--

LOCK TABLES `invoice` WRITE;
/*!40000 ALTER TABLE `invoice` DISABLE KEYS */;
INSERT INTO `invoice` VALUES ('3658886893','96T47PQ9VYKPJRD','2020-02-20',6741,'1003',0,2247,0,'',1),('9741767628','96T47PQ9VYKPJRD','2020-02-01',25510.5,'1000',0,8503.5,0,'',1),('4982866655','96T47PQ9VYKPJRD','2020-02-01',0,'1001',0,11338,0,'',1),('9165245621','96T47PQ9VYKPJRD','2020-02-14',11119.5,'1002',0,3706.5,0,'',1),('5135984974','96T47PQ9VYKPJRD','2020-02-20',0,'1004',0,2347,0,'',1);
/*!40000 ALTER TABLE `invoice` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `invoice_details`
--

DROP TABLE IF EXISTS `invoice_details`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `invoice_details` (
  `invoice_details_id` varchar(100) NOT NULL,
  `invoice_id` varchar(100) NOT NULL,
  `product_id` varchar(100) NOT NULL,
  `quantity` float NOT NULL,
  `rate` float NOT NULL,
  `supplier_rate` float DEFAULT NULL,
  `total_price` float NOT NULL,
  `discount` float DEFAULT NULL,
  `discount_per` varchar(15) DEFAULT NULL,
  `tax` float DEFAULT NULL,
  `paid_amount` float DEFAULT '0',
  `due_amount` float NOT NULL DEFAULT '0',
  `status` int(2) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `invoice_details`
--

LOCK TABLES `invoice_details` WRITE;
/*!40000 ALTER TABLE `invoice_details` DISABLE KEYS */;
INSERT INTO `invoice_details` VALUES ('942889924366385','9741767628','33776845',3,599,419.3,1797,0.01,'25',0,0,25510.5,1),('946444473926784','9741767628','69472824',3,1450,1015,4350,0.01,'25',0,0,25510.5,1),('578581454429282','9741767628','79774872',3,1250,875,3750,0.01,'25',0,0,25510.5,1),('654971486447783','9741767628','62955453',3,1450,1015,4350,0.01,'25',0,0,25510.5,1),('579223651852421','9741767628','21826863',3,599,419.3,1797,0.01,'25',0,0,25510.5,1),('824113995432159','9741767628','96482874',3,599,419.3,1797,0.01,'25',0,0,25510.5,1),('517428773758991','9741767628','59648166',3,599,419.3,1797,0.01,'25',0,0,25510.5,1),('983483682124229','9741767628','38998239',3,599,419.3,1797,0.01,'25',0,0,25510.5,1),('279822688954844','9741767628','41211877',3,599,419.3,1797,0.01,'25',0,0,25510.5,1),('832171448549962','9741767628','45855167',3,599,419.3,1797,0.01,'25',0,0,25510.5,1),('889861938833226','9741767628','66953714',3,599,419.3,1797,0.01,'25',0,0,25510.5,1),('699717741695692','9741767628','18968634',3,599,419.3,1797,0.01,'25',0,0,25510.5,1),('789666669543764','9741767628','29197187',3,599,419.3,1797,0.01,'25',0,0,25510.5,1),('424494178138597','9741767628','14559614',3,599,419.3,1797,0.01,'25',0,0,25510.5,1),('665632297142328','9741767628','42476663',3,599,419.3,1797,0.01,'25',0,0,25510.5,1),('513446874198665','4982866655','96118932',1,1450,1015,1450,0.01,'100',0,0,0,1),('375519817983648','4982866655','19722853',1,1250,875,1250,0.01,'100',0,0,0,1),('288433129529386','4982866655','96724851',1,1450,1015,1450,0.01,'100',0,0,0,1),('813353343941315','4982866655','64231489',1,599,419.3,599,0.01,'100',0,0,0,1),('123866267219147','4982866655','39966796',1,599,419.3,599,0.01,'100',0,0,0,1),('628854277562682','4982866655','75841999',1,599,419.3,599,0.01,'100',0,0,0,1),('184479996716619','4982866655','27787243',1,599,419.3,599,0.01,'100',0,0,0,1),('471366673438697','4982866655','43823574',1,599,419.3,599,0.01,'100',0,0,0,1),('796244143613374','4982866655','35387537',1,599,419.3,599,0.01,'100',0,0,0,1),('221346894246995','4982866655','51814898',1,599,419.3,599,0.01,'100',0,0,0,1),('775389783954371','4982866655','24432454',1,599,419.3,599,0.01,'100',0,0,0,1),('682725315729428','4982866655','59892815',1,599,419.3,599,0.01,'100',0,0,0,1),('985424392686924','4982866655','82991274',1,599,419.3,599,0.01,'100',0,0,0,1),('982254785356288','4982866655','55426419',1,599,419.3,599,0.01,'100',0,0,0,1),('977528857116797','4982866655','72945797',1,599,419.3,599,0.01,'100',0,0,0,1),('187417743172877','9165245621','31973839',3,449,314.3,1347,0.01,'25',0,0,11119.5,1),('328571477991552','9165245621','52812878',3,449,314.3,1347,0.01,'25',0,0,11119.5,1),('561211875273369','9165245621','12495721',3,649,454.3,1947,0.01,'25',0,0,11119.5,1),('991757795484549','9165245621','19165852',3,999,699.3,2997,0.01,'25',0,0,11119.5,1),('786816893411637','9165245621','38998239',2,599,419.3,1198,0.01,'25',0,0,11119.5,1),('536611479828373','9165245621','45855167',4,599,419.3,2396,0.01,'25',0,0,11119.5,1),('352832522852815','9165245621','29197187',4,599,419.3,2396,0.01,'25',0,0,11119.5,1),('568238266257175','9165245621','14559614',2,599,419.3,1198,0.01,'25',0,0,11119.5,1),('336786134728221','5135984974','83847398',1,649,454.3,649,0.01,'100',0,0,0,1),('931358393137645','3658886893','12495721',6,649,454.3,3894,0.01,'25',0,0,6741,1),('414615971868458','3658886893','19165852',3,999,699.3,2997,0.01,'25',0,0,6741,1),('641776515818672','3658886893','47575149',3,699,489.3,2097,0.01,'25',0,0,6741,1),('414594586855128','5135984974','36826337',1,699,489.3,699,0.01,'100',0,0,0,1),('929597911598996','5135984974','45772378',1,999,699.3,999,0.01,'100',0,0,0,1);
/*!40000 ALTER TABLE `invoice_details` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `language`
--

DROP TABLE IF EXISTS `language`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `language` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `phrase` text NOT NULL,
  `english` text,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=568 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `language`
--

LOCK TABLES `language` WRITE;
/*!40000 ALTER TABLE `language` DISABLE KEYS */;
INSERT INTO `language` VALUES (1,'user_profile','User Profile'),(2,'setting','Setting'),(3,'language','Language'),(4,'manage_users','Manage Users'),(5,'add_user','Add User'),(6,'manage_company','Manage Company'),(7,'web_settings','Software Settings'),(8,'manage_accounts','Manage Accounts'),(9,'create_accounts','Create Account'),(10,'manage_bank','Manage Bank'),(11,'add_new_bank','Add New Bank'),(12,'settings','Bank'),(13,'closing_report','Closing Report'),(14,'closing','Closing'),(15,'cheque_manager','Cheque Manager'),(16,'accounts_summary','Accounts Summary'),(17,'expense','Expense'),(18,'income','Income'),(19,'accounts','Accounts'),(20,'stock_report','Stock Report'),(21,'stock','Stock'),(22,'pos_invoice','POS Invoice'),(23,'manage_invoice','Manage Invoice '),(24,'new_invoice','New Invoice'),(25,'invoice','Invoice'),(26,'manage_purchase','Manage Purchase'),(27,'add_purchase','Add Purchase'),(28,'purchase','Purchase'),(29,'paid_customer','Paid Customer'),(30,'manage_customer','Manage Customer'),(31,'add_customer','Add Customer'),(32,'customer','Customer'),(33,'supplier_payment_actual','Supplier Payment Ledger'),(34,'supplier_sales_summary','Supplier Sales Summary'),(35,'supplier_sales_details','Supplier Sales Details'),(36,'supplier_ledger','Supplier Ledger'),(37,'manage_supplier','Manage Supplier'),(38,'add_supplier','Add Supplier'),(39,'supplier','Supplier'),(40,'product_statement','Product Statement'),(41,'manage_product','Manage Product'),(42,'add_product','Add Product'),(43,'product','Product'),(44,'manage_category','Manage Category'),(45,'add_category','Add Category'),(46,'category','Category'),(47,'sales_report_product_wise','Sales Report (Product Wise)'),(48,'purchase_report','Purchase Report'),(49,'sales_report','Sales Report'),(50,'todays_report','Todays Report'),(51,'report','Report'),(52,'dashboard','Dashboard'),(53,'online','Online'),(54,'logout','Logout'),(55,'change_password','Change Password'),(56,'total_purchase','Total Purchase'),(57,'total_amount','Total Amount'),(58,'supplier_name','Supplier Name'),(59,'invoice_no','Invoice No'),(60,'purchase_date','Purchase Date'),(61,'todays_purchase_report','Todays Purchase Report'),(62,'total_sales','Total Sales'),(63,'customer_name','Customer Name'),(64,'sales_date','Sales Date'),(65,'todays_sales_report','Todays Sales Report'),(66,'home','Home'),(67,'todays_sales_and_purchase_report','Todays sales and purchase report'),(68,'total_ammount','Total Amount'),(69,'rate','Rate'),(70,'product_model','Product Model'),(71,'product_name','Product Name'),(72,'search','Search'),(73,'end_date','End Date'),(74,'start_date','Start Date'),(75,'total_purchase_report','Total Purchase Report'),(76,'total_sales_report','Total Sales Report'),(77,'total_seles','Total Sales'),(78,'all_stock_report','All Stock Report'),(79,'search_by_product','Search By Product'),(80,'date','Date'),(81,'print','Print'),(82,'stock_date','Stock Date'),(83,'print_date','Print Date'),(84,'sales','Sales'),(85,'price','Price'),(86,'sl','SL.'),(87,'add_new_category','Add new category'),(88,'category_name','Category Name'),(89,'save','Save'),(90,'delete','Delete'),(91,'update','Update'),(92,'action','Action'),(93,'manage_your_category','Manage your category '),(94,'category_edit','Category Edit'),(95,'status','Status'),(96,'active','Active'),(97,'inactive','Inactive'),(98,'save_changes','Save Changes'),(99,'save_and_add_another','Save And Add Another'),(100,'model','Model'),(101,'supplier_price','Supplier Price'),(102,'sell_price','Sale Price'),(103,'image','Image'),(104,'select_one','Select One'),(105,'details','Details'),(106,'new_product','New Product'),(107,'add_new_product','Add new product'),(108,'barcode','Barcode'),(109,'qr_code','Qr-Code'),(110,'product_details','Product Details'),(111,'manage_your_product','Manage your product'),(112,'product_edit','Product Edit'),(113,'edit_your_product','Edit your product'),(114,'cancel','Cancel'),(115,'incl_vat','Incl. Vat'),(116,'money','TK'),(117,'grand_total','Grand Total'),(118,'quantity','Qnty'),(119,'product_report','Product Report'),(120,'product_sales_and_purchase_report','Product sales and purchase report'),(121,'previous_stock','Previous Stock'),(122,'out','Out'),(123,'in','In'),(124,'to','To'),(125,'previous_balance','Previous Credit Balance'),(126,'customer_address','Customer Address'),(127,'customer_mobile','Customer Mobile'),(128,'customer_email','Customer Email'),(129,'add_new_customer','Add new customer'),(130,'balance','Balance'),(131,'mobile','Mobile'),(132,'address','Address'),(133,'manage_your_customer','Manage your customer'),(134,'customer_edit','Customer Edit'),(135,'paid_customer_list','Paid Customer List'),(136,'ammount','Amount'),(137,'customer_ledger','Customer Ledger'),(138,'manage_customer_ledger','Manage Customer Ledger'),(139,'customer_information','Customer Information'),(140,'debit_ammount','Debit Amount'),(141,'credit_ammount','Credit Amount'),(142,'balance_ammount','Balance Amount'),(143,'receipt_no','Receipt NO'),(144,'description','Description'),(145,'debit','Debit'),(146,'credit','Credit'),(147,'item_information','Item Information'),(148,'total','Total'),(149,'please_select_supplier','Please Select Supplier'),(150,'submit','Submit'),(151,'submit_and_add_another','Submit And Add Another One'),(152,'add_new_item','Add New Item'),(153,'manage_your_purchase','Manage your purchase'),(154,'purchase_edit','Purchase Edit'),(155,'purchase_ledger','Purchase Ledger'),(156,'invoice_information','Invoice Information'),(157,'paid_ammount','Paid Amount'),(158,'discount','Dis./Pcs.'),(159,'save_and_paid','Save And Paid'),(160,'payee_name','Payee Name'),(161,'manage_your_invoice','Manage your invoice'),(162,'invoice_edit','Invoice Edit'),(163,'new_pos_invoice','New POS invoice'),(164,'add_new_pos_invoice','Add new pos invoice'),(165,'product_id','Product ID'),(166,'paid_amount','Paid Amount'),(167,'authorised_by','Authorised By'),(168,'checked_by','Checked By'),(169,'received_by','Received By'),(170,'prepared_by','Prepared By'),(171,'memo_no','Memo No'),(172,'website','Website'),(173,'email','Email'),(174,'invoice_details','Invoice Details'),(175,'reset','Reset'),(176,'payment_account','Payment Account'),(177,'bank_name','Bank Name'),(178,'cheque_or_pay_order_no','Cheque/Pay Order No'),(179,'payment_type','Payment Type'),(180,'payment_from','Payment From'),(181,'payment_date','Payment Date'),(182,'add_income','Add Income'),(183,'cash','Cash'),(184,'cheque','Cheque'),(185,'pay_order','Pay Order'),(186,'payment_to','Payment To'),(187,'total_outflow_ammount','Total Expense Amount'),(188,'transections','Transections'),(189,'accounts_name','Accounts Name'),(190,'outflow_report','Expense Report'),(191,'inflow_report','Income Report'),(192,'all','All'),(193,'account','Account'),(194,'from','From'),(195,'account_summary_report','Account Summary Report'),(196,'search_by_date','Search By Date'),(197,'cheque_no','Cheque No'),(198,'name','Name'),(199,'closing_account','Closing Account'),(200,'close_your_account','Close your account'),(201,'last_day_closing','Last Day Closing'),(202,'cash_in','Cash In'),(203,'cash_out','Cash Out'),(204,'cash_in_hand','Cash In Hand'),(205,'add_new_bank','Add New Bank'),(206,'day_closing','Day Closing'),(207,'account_closing_report','Account Closing Report'),(208,'last_day_ammount','Last Day Amount'),(209,'adjustment','Adjustment'),(210,'pay_type','Pay Type'),(211,'customer_or_supplier','Customer,Supplier Or Others'),(212,'transection_id','Transections ID'),(213,'accounts_summary_report','Accounts Summary Report'),(214,'bank_list','Bank List'),(215,'bank_edit','Bank Edit'),(216,'debit_plus','Debit (+)'),(217,'credit_minus','Credit (-)'),(218,'account_name','Account Name'),(219,'account_type','Account Type'),(220,'account_real_name','Account Real Name'),(221,'manage_account','Manage Account'),(222,'company_name','Niha International'),(223,'edit_your_company_information','Edit your company information'),(224,'company_edit','Company Edit'),(225,'admin','Admin'),(226,'user','User'),(227,'password','Password'),(228,'last_name','Last Name'),(229,'first_name','First Name'),(230,'add_new_user_information','Add new user information'),(231,'user_type','User Type'),(232,'user_edit','User Edit'),(233,'rtr','RTR'),(234,'ltr','LTR'),(235,'ltr_or_rtr','LTR/RTR'),(236,'footer_text','Footer Text'),(237,'favicon','Favicon'),(238,'logo','Logo'),(239,'update_setting','Update Setting'),(240,'update_your_web_setting','Update your web setting'),(241,'login','Login'),(242,'your_strong_password','Your strong password'),(243,'your_unique_email','Your unique email'),(244,'please_enter_your_login_information','Please enter your login information.'),(245,'update_profile','Update Profile'),(246,'your_profile','Your Profile'),(247,'re_type_password','Re-Type Password'),(248,'new_password','New Password'),(249,'old_password','Old Password'),(250,'new_information','New Information'),(251,'old_information','Old Information'),(252,'change_your_information','Change your information'),(253,'change_your_profile','Change your profile'),(254,'profile','Profile'),(255,'wrong_username_or_password','Wrong User Name Or Password !'),(256,'successfully_updated','Successfully Updated.'),(257,'blank_field_does_not_accept','Blank Field Does Not Accept !'),(258,'successfully_changed_password','Successfully changed password.'),(259,'you_are_not_authorised_person','You are not authorised person !'),(260,'password_and_repassword_does_not_match','Passwor and re-password does not match !'),(261,'new_password_at_least_six_character','New Password At Least 6 Character.'),(262,'you_put_wrong_email_address','You put wrong email address !'),(263,'cheque_ammount_asjusted','Cheque amount adjusted.'),(264,'successfully_payment_paid','Successfully Payment Paid.'),(265,'successfully_added','Successfully Added.'),(266,'successfully_updated_2_closing_ammount_not_changeale','Successfully Updated -2. Note: Closing Amount Not Changeable.'),(267,'successfully_payment_received','Successfully Payment Received.'),(268,'already_inserted','Already Inserted !'),(269,'successfully_delete','Successfully Delete.'),(270,'successfully_created','Successfully Created.'),(271,'logo_not_uploaded','Logo not uploaded !'),(272,'favicon_not_uploaded','Favicon not uploaded !'),(273,'supplier_mobile','Supplier Mobile'),(274,'supplier_address','Supplier Address'),(275,'supplier_details','Supplier Details'),(276,'add_new_supplier','Add New Supplier'),(277,'manage_suppiler','Manage Supplier'),(278,'manage_your_supplier','Manage your supplier'),(279,'manage_supplier_ledger','Manage supplier ledger'),(280,'invoice_id','Invoice ID'),(281,'deposite_id','Deposite ID'),(282,'supplier_actual_ledger','Supplier Payment Ledger'),(283,'supplier_information','Supplier Information'),(284,'event','Event'),(285,'add_new_income','Add New Income'),(286,'add_expese','Add Expense'),(287,'add_new_expense','Add New Expense'),(288,'total_inflow_ammount','Total Income Amount'),(289,'create_new_invoice','Create New Invoice'),(290,'create_pos_invoice','Create POS Invoice'),(291,'total_profit','Total Profit'),(292,'monthly_progress_report','Monthly Progress Report'),(293,'total_invoice','Total Invoice'),(294,'account_summary','Account Summary'),(295,'total_supplier','Total Supplier'),(296,'total_product','Total Product'),(297,'total_customer','Total Customer'),(298,'supplier_edit','Supplier Edit'),(299,'add_new_invoice','Add New Invoice'),(300,'add_new_purchase','Add new purchase'),(301,'currency','Currency'),(302,'currency_position','Currency Position'),(303,'left','Left'),(304,'right','Right'),(305,'add_tax','Add Tax'),(306,'manage_tax','Manage Tax'),(307,'add_new_tax','Add new tax'),(308,'enter_tax','Enter Tax'),(309,'already_exists','Already Exists !'),(310,'successfully_inserted','Successfully Inserted.'),(311,'tax','Tax'),(312,'tax_edit','Tax Edit'),(313,'product_not_added','Product not added !'),(314,'total_tax','Total Tax'),(315,'manage_your_supplier_details','Manage your supplier details.'),(316,'invoice_description','Lorem Ipsum is sim ply dummy Lorem Ipsum is simply dummy Lorem Ipsum is simply dummy Lorem Ipsum is simply dummy Lorem Ipsum is simply dummy Lorem Ipsum is sim ply dummy Lorem Ipsum is simply dummy Lorem Ipsum is simply dummy Lorem Ipsum is simply dummy Lorem Ipsum is simply dummy'),(317,'thank_you_for_choosing_us','Thank you very much for choosing us.'),(318,'billing_date','Billing Date'),(319,'billing_to','Billing To'),(320,'billing_from','Billing From'),(321,'you_cant_delete_this_product','Sorry !!  You can\'t delete this product.This product already used in calculation system!'),(322,'old_customer','Old Customer'),(323,'new_customer','New Customer'),(324,'new_supplier','New Supplier'),(325,'old_supplier','Old Supplier'),(326,'credit_customer','Credit Customer'),(327,'account_already_exists','This Account Already Exists !'),(328,'edit_income','Edit Income'),(329,'you_are_not_access_this_part','You are not authorised person !'),(330,'account_edit','Account Edit'),(331,'due','Due'),(332,'expense_edit','Expense Edit'),(333,'please_select_customer','Please select customer !'),(334,'profit_report','Profit Report (Invoice Wise)'),(335,'total_profit_report','Total profit report'),(336,'please_enter_valid_captcha','Please enter valid captcha.'),(337,'category_not_selected','Category not selected.'),(338,'supplier_not_selected','Supplier not selected.'),(339,'please_select_product','Please select product.'),(340,'product_model_already_exist','Product model already exist !'),(341,'invoice_logo','Invoice Logo'),(342,'available_qnty','Av. Qnty.'),(343,'you_can_not_buy_greater_than_available_cartoon','You can not select grater than available cartoon !'),(344,'customer_details','Customer details'),(345,'manage_customer_details','Manage customer details.'),(346,'site_key','Captcha Site Key'),(347,'secret_key','Captcha Secret Key'),(348,'captcha','Captcha'),(349,'cartoon_quantity','Cartoon Quantity'),(350,'total_cartoon','Total Cartoon'),(351,'cartoon','Cartoon'),(352,'item_cartoon','Item/Cartoon'),(353,'product_and_supplier_did_not_match','Product and supplier did not match !'),(354,'if_you_update_purchase_first_select_supplier_then_product_and_then_quantity','If you update purchase,first select supplier then product and then update qnty.'),(355,'item','Item'),(356,'manage_your_credit_customer','Manage your credit customer'),(357,'total_quantity','Total Quantity'),(358,'quantity_per_cartoon','Quantity per cartoon'),(359,'barcode_qrcode_scan_here','Barcode or QR-code scan here'),(360,'synchronizer_setting','Synchronizer Setting'),(361,'data_synchronizer','Data Synchronizer'),(362,'hostname','Host name'),(363,'username','User Name'),(364,'ftp_port','FTP Port'),(365,'ftp_debug','FTP Debug'),(366,'project_root','Project Root'),(367,'please_try_again','Please try again'),(368,'save_successfully','Save successfully'),(369,'synchronize','Synchronize'),(370,'internet_connection','Internet Connection'),(371,'outgoing_file','Outgoing File'),(372,'incoming_file','Incoming File'),(373,'ok','Ok'),(374,'not_available','Not Available'),(375,'available','Available'),(376,'download_data_from_server','Download data from server'),(377,'data_import_to_database','Data import to database'),(378,'data_upload_to_server','Data uplod to server'),(379,'please_wait','Please Wait'),(380,'ooops_something_went_wrong','Oooops Something went wrong !'),(381,'upload_successfully','Upload successfully'),(382,'unable_to_upload_file_please_check_configuration','Unable to upload file please check configuration'),(383,'please_configure_synchronizer_settings','Please configure synchronizer settings'),(384,'download_successfully','Download successfully'),(385,'unable_to_download_file_please_check_configuration','Unable to download file please check configuration'),(386,'data_import_first','Data import past'),(387,'data_import_successfully','Data import successfully'),(388,'unable_to_import_data_please_check_config_or_sql_file','Unable to import data please check config or sql file'),(389,'total_sale_ctn','Total Sale Ctn'),(390,'in_qnty','In Qnty.'),(391,'out_qnty','Out Qnty.'),(392,'stock_report_supplier_wise','Stock Report (Supplier Wise)'),(393,'all_stock_report_supplier_wise','Stock Report (Suppler Wise)'),(394,'select_supplier','Select Supplier'),(395,'stock_report_product_wise','Stock Report (Product Wise)'),(396,'phone','Phone'),(397,'select_product','Select Product'),(398,'in_quantity','In Qnty.'),(399,'out_quantity','Out Qnty.'),(400,'in_taka','In TK.'),(401,'out_taka','Out TK.'),(402,'commission','Commission'),(403,'generate_commission','Generate Commssion'),(404,'commission_rate','Commission Rate'),(405,'total_ctn','Total Ctn.'),(406,'per_pcs_commission','Per PCS Commission'),(407,'total_commission','Total Commission'),(408,'enter','Enter'),(409,'please_add_walking_customer_for_default_customer','Please add \'Walking Customer\' for default customer.'),(410,'supplier_ammount','Supplier Amount'),(411,'my_sale_ammount','My Sale Amount'),(412,'signature_pic','Signature Picture'),(413,'branch','Branch'),(414,'ac_no','A/C Number'),(415,'ac_name','A/C Name'),(416,'bank_transaction','Bank Transaction'),(417,'bank','Bank'),(418,'withdraw_deposite_id','Withdraw / Deposite ID'),(419,'bank_ledger','Bank Ledger'),(420,'note_name','Note Name'),(421,'pcs','Pcs.'),(422,'1','1'),(423,'2','2'),(424,'5','5'),(425,'10','10'),(426,'20','20'),(427,'50','50'),(428,'100','100'),(429,'500','500'),(430,'1000','1000'),(431,'total_discount','Total Discount'),(432,'product_not_found','Product not found !'),(433,'this_is_not_credit_customer','This is not credit customer !'),(434,'personal_loan','Office Loan'),(435,'add_person','Add Person'),(436,'add_loan','Add Loan'),(437,'add_payment','Add Payment'),(438,'manage_person','Manage Person'),(439,'personal_edit','Person Edit'),(440,'person_ledger','Person Ledger'),(441,'backup_restore','Backup '),(442,'database_backup','Database backup'),(443,'file_information','File information'),(444,'filename','Filename'),(445,'size','Size'),(446,'backup_date','Backup date'),(447,'backup_now','Backup now'),(448,'restore_now','Restore now'),(449,'are_you_sure','Are you sure ?'),(450,'download','Download'),(451,'backup_and_restore','Backup'),(452,'backup_successfully','Backup successfully'),(453,'delete_successfully','Delete successfully'),(454,'stock_ctn','Stock/Qnt'),(455,'unit','Unit'),(456,'meter_m','Meter (M)'),(457,'piece_pc','Piece (Pc)'),(458,'kilogram_kg','Kilogram (Kg)'),(459,'stock_cartoon','Stock Cartoon'),(460,'add_product_csv','Add Product (CSV)'),(461,'import_product_csv','Import product (CSV)'),(462,'close','Close'),(463,'download_example_file','Download example file.'),(464,'upload_csv_file','Upload CSV File'),(465,'csv_file_informaion','CSV File Information'),(466,'out_of_stock','Out Of Stock'),(467,'others','Others'),(468,'full_paid','Full Paid'),(469,'successfully_saved','Your Data Successfully Saved'),(470,'manage_loan','Manage Loan'),(471,'receipt','Receipt'),(472,'payment','Payment'),(473,'cashflow','Daily Cash Flow'),(474,'signature','Signature'),(475,'supplier_reports','Supplier Reports'),(476,'generate','Generate'),(477,'todays_overview','Todays Overview'),(478,'last_sales','Last Sales'),(479,'manage_transaction','Manage Transaction'),(480,'daily_summary','Daily Summary'),(481,'daily_cash_flow','Daily Cash Flow'),(482,'custom_report','Custom Report'),(483,'transaction','Transaction'),(484,'receipt_amount','Receipt Amount'),(485,'transaction_details_datewise','Transaction Details Datewise'),(486,'cash_closing','Cash Closing'),(487,'you_can_not_buy_greater_than_available_qnty','You can not buy greater than available qnty.'),(488,'supplier_id','Supplier ID'),(489,'category_id','Category ID'),(490,'select_report','Select Report'),(491,'supplier_summary','Supplier summary'),(492,'sales_payment_actual','Sales payment actual'),(493,'today_already_closed','Today already closed.'),(494,'root_account','Root Account'),(495,'office','Office'),(496,'loan','Loan'),(497,'transaction_mood','Transaction Mood'),(498,'select_account','Select Account'),(499,'add_receipt','Add Receipt'),(500,'update_transaction','Update Transaction'),(501,'no_stock_found','No Stock Found !'),(502,'admin_login_area','Admin Login Area'),(503,'print_qr_code','Print QR Code'),(504,'discount_type','Discount Type'),(505,'discount_percentage','Discount'),(506,'fixed_dis','Fixed Dis.'),(507,'return','Return'),(508,'stock_return_list','Stock Return List'),(509,'wastage_return_list','Wastage Return List'),(510,'return_invoice','Invoice Return'),(511,'sold_qty','Sold Qty'),(512,'ret_quantity','Return Qty'),(513,'deduction','Deduction'),(514,'check_return','Check Return'),(515,'reason','Reason'),(516,'usablilties','Usability'),(517,'adjs_with_stck','Adjust With Stock'),(518,'return_to_supplier','Return To Supplier'),(519,'wastage','Wastage'),(520,'to_deduction','Total Deduction '),(521,'nt_return','Net Return Amount'),(522,'return_list','Return List'),(523,'add_return','Add Return'),(524,'per_qty','Purchase Qty'),(525,'return_supplier','Supplier Return'),(526,'stock_purchase','Stock Purchase Price'),(527,'stock_sale','Stock Sale Price'),(528,'supplier_return','Supplier Return'),(529,'purchase_id','Purchase ID'),(530,'return_id','Return ID'),(531,'supplier_return_list','Supplier Return List'),(532,'c_r_slist','Stock Return Stock'),(533,'wastage_list','Wastage List'),(534,'please_input_correct_invoice_id','Please Input a Correct Invoice ID'),(535,'please_input_correct_purchase_id','Please Input Your Correct  Purchase ID'),(536,'add_more','Add More'),(537,'prouct_details','Product Details'),(538,'prouct_detail','Product Details'),(539,'stock_return','Stock Return'),(540,'choose_transaction','Select Transaction'),(541,'transection_category','Select  Category'),(542,'transaction_categry','Select Category'),(543,'search_supplier','Search Supplier'),(544,'customer_id','Customer ID'),(545,'search_customer','Search Customer Invoice'),(546,'serial_no','Serial Number'),(547,'item_discount','Item Discount'),(548,'invoice_discount','Invoice Discount'),(549,'add_unit','Add Unit'),(550,'manage_unit','Manage Unit'),(551,'add_new_unit','Add New Unit'),(552,'unit_name','Unit Name'),(553,'payment_amount','Payment Amount'),(554,'manage_your_unit','Manage Your Unit'),(555,'unit_id','Unit ID'),(556,'unit_edit','Unit Edit'),(557,'vat','Vat'),(558,'sales_report_category_wise','Sales Report (Category wise)'),(559,'purchase_report_category_wise','Purchase Report (Category wise)'),(560,'category_wise_purchase_report','Category wise purchase report'),(561,'category_wise_sales_report','Category wise sales report'),(562,'best_sale_product','Best Sale Product'),(563,'all_best_sales_product','All Best Sales Products'),(564,'todays_customer_receipt','Todays Customer Receipt'),(565,'not_found','Record not found'),(566,'collection','Collection'),(567,'increment','Increment');
/*!40000 ALTER TABLE `language` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notes`
--

DROP TABLE IF EXISTS `notes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `notes` (
  `note_id` int(11) NOT NULL AUTO_INCREMENT,
  `cash_date` varchar(20) NOT NULL,
  `1000n` varchar(11) NOT NULL,
  `500n` varchar(11) NOT NULL,
  `100n` varchar(11) NOT NULL,
  `50n` varchar(11) NOT NULL,
  `20n` varchar(11) NOT NULL,
  `10n` varchar(11) NOT NULL,
  `5n` varchar(11) NOT NULL,
  `2n` varchar(11) NOT NULL,
  `1n` varchar(30) NOT NULL,
  `grandtotal` varchar(30) NOT NULL,
  PRIMARY KEY (`note_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notes`
--

LOCK TABLES `notes` WRITE;
/*!40000 ALTER TABLE `notes` DISABLE KEYS */;
INSERT INTO `notes` VALUES (1,'2020-01-24','20','20','10','','','','','','','31000');
/*!40000 ALTER TABLE `notes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payment_trans`
--

DROP TABLE IF EXISTS `payment_trans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `payment_trans` (
  `transection_id` varchar(200) NOT NULL,
  `tracing_id` varchar(200) NOT NULL,
  `payment_type` varchar(10) NOT NULL,
  `date` varchar(50) DEFAULT NULL,
  `amount` float NOT NULL,
  `description` varchar(255) NOT NULL,
  `status` int(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payment_trans`
--

LOCK TABLES `payment_trans` WRITE;
/*!40000 ALTER TABLE `payment_trans` DISABLE KEYS */;
/*!40000 ALTER TABLE `payment_trans` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `person_information`
--

DROP TABLE IF EXISTS `person_information`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `person_information` (
  `person_id` varchar(50) NOT NULL,
  `person_name` varchar(50) NOT NULL,
  `person_phone` varchar(50) NOT NULL,
  `person_address` text NOT NULL,
  `status` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `person_information`
--

LOCK TABLES `person_information` WRITE;
/*!40000 ALTER TABLE `person_information` DISABLE KEYS */;
/*!40000 ALTER TABLE `person_information` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `person_ledger`
--

DROP TABLE IF EXISTS `person_ledger`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `person_ledger` (
  `transaction_id` varchar(50) NOT NULL,
  `person_id` varchar(50) NOT NULL,
  `date` varchar(50) NOT NULL,
  `debit` float NOT NULL,
  `credit` float NOT NULL,
  `details` text NOT NULL,
  `status` int(11) NOT NULL COMMENT '1=no paid,2=paid',
  `id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `person_ledger`
--

LOCK TABLES `person_ledger` WRITE;
/*!40000 ALTER TABLE `person_ledger` DISABLE KEYS */;
/*!40000 ALTER TABLE `person_ledger` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `personal_loan`
--

DROP TABLE IF EXISTS `personal_loan`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `personal_loan` (
  `per_loan_id` int(11) NOT NULL AUTO_INCREMENT,
  `transaction_id` varchar(30) NOT NULL,
  `person_id` varchar(30) NOT NULL,
  `debit` float DEFAULT NULL,
  `credit` float NOT NULL,
  `date` varchar(30) NOT NULL,
  `details` varchar(100) NOT NULL,
  `status` int(11) NOT NULL COMMENT '1=no paid,2=paid',
  PRIMARY KEY (`per_loan_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `personal_loan`
--

LOCK TABLES `personal_loan` WRITE;
/*!40000 ALTER TABLE `personal_loan` DISABLE KEYS */;
/*!40000 ALTER TABLE `personal_loan` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pesonal_loan_information`
--

DROP TABLE IF EXISTS `pesonal_loan_information`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pesonal_loan_information` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `person_id` varchar(50) NOT NULL,
  `person_name` varchar(50) NOT NULL,
  `person_phone` varchar(30) NOT NULL,
  `person_address` text NOT NULL,
  `status` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pesonal_loan_information`
--

LOCK TABLES `pesonal_loan_information` WRITE;
/*!40000 ALTER TABLE `pesonal_loan_information` DISABLE KEYS */;
/*!40000 ALTER TABLE `pesonal_loan_information` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_category`
--

DROP TABLE IF EXISTS `product_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `product_category` (
  `category_id` varchar(255) DEFAULT NULL,
  `category_name` varchar(255) DEFAULT NULL,
  `status` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_category`
--

LOCK TABLES `product_category` WRITE;
/*!40000 ALTER TABLE `product_category` DISABLE KEYS */;
INSERT INTO `product_category` VALUES ('9FVJHH7X8BAIIND','FACE ',1),('79Q3KB7P9P832FS','EYES',1),('P7CFSTEKJ8KXMG9','LIPS',1),('W29QGM6LFGMNK84','NAILS',1),('499RMJEH12F5H9A','HAIR',1);
/*!40000 ALTER TABLE `product_category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_information`
--

DROP TABLE IF EXISTS `product_information`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `product_information` (
  `product_id` varchar(100) NOT NULL,
  `category_id` varchar(255) DEFAULT NULL,
  `product_name` varchar(255) NOT NULL,
  `price` float NOT NULL,
  `unit` varchar(50) DEFAULT NULL,
  `tax` float DEFAULT NULL COMMENT 'Tax in %',
  `serial_no` varchar(50) DEFAULT NULL,
  `product_model` varchar(100) NOT NULL,
  `product_details` varchar(255) NOT NULL,
  `image` varchar(255) DEFAULT NULL,
  `status` int(2) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_information`
--

LOCK TABLES `product_information` WRITE;
/*!40000 ALTER TABLE `product_information` DISABLE KEYS */;
INSERT INTO `product_information` VALUES ('57317359','0','LAPTOP',15000,'Piece',0,'12345678','INF0001','','http://202.166.166.100/oerp/my-assets/image/product.png',1),('63511867','9FVJHH7X8BAIIND','Technic Contour Stix',880,'Piece',0,'','5021769257097','','http://202.166.166.100/oerp/my-assets/image/product/contour_stix.jpg',1),('42225661','9FVJHH7X8BAIIND','Technic Corrector Stix',880,'Piece',0,'','5021769267225','','http://202.166.166.100/oerp/my-assets/image/product/1.jpg',1),('62955453','9FVJHH7X8BAIIND','Technic Colour Fix Cream Foundation  Contouring Palette',1450,'Piece',0,'','5021769255055','','http://202.166.166.100/oerp/my-assets/image/product/cream_foundation_and_contouring_palette.jpg',1),('79774872','9FVJHH7X8BAIIND','Technic Corrector and Contour Palette',1250,'Piece',0,'','5021769267058','','http://202.166.166.100/oerp/my-assets/image/product/Corrector_and_Contour_Palette.jpg',1),('69472824','9FVJHH7X8BAIIND','Technic Colour Fix Blush Palette         ',1450,'Piece',0,'','5021769257059','','http://202.166.166.100/oerp/my-assets/image/product/blush_palette.jpg',1),('25558411','9FVJHH7X8BAIIND','Technic Get Gorgeous Pink Sparkle Highlighter Powder ',850,'Piece',0,'','5021769267300','','http://202.166.166.100/oerp/my-assets/image/product/Pink_Sparkle.jpg',1),('23874322','9FVJHH7X8BAIIND','Technic Get Gorgeous 24CT Gold Highlighter Powder',850,'Piece',0,'','5021769267324','','http://202.166.166.100/oerp/my-assets/image/product/24ct_Gold.jpg',1),('65873862','9FVJHH7X8BAIIND','Technic Get Gorgeous highlighting Powder',850,'Piece',0,'','5021769257035','','http://202.166.166.100/oerp/my-assets/image/product/Highlighting_powder.jpg',1),('66222482','9FVJHH7X8BAIIND','Technic Super Fine Matte Blusher',750,'Piece',0,'','5021769267010','','http://202.166.166.100/oerp/my-assets/image/product/Matte_blusher_2.jpg',1),('58296557','9FVJHH7X8BAIIND','Technic High lights',810,'Piece',0,'','5021769207016','','http://202.166.166.100/oerp/my-assets/image/product/highlights.jpg',1),('32958561','9FVJHH7X8BAIIND','Technic Mega Blush',810,'Piece',0,'','5021769247074','','http://202.166.166.100/oerp/my-assets/image/product/Mega_Blush.jpg',1),('65346714','9FVJHH7X8BAIIND','Technic Matte Mineral Foundation - Buff',1100,'Piece',0,'','5021769267126','','http://202.166.166.100/oerp/my-assets/image/product/ty_(1).jpg',1),('32878258','9FVJHH7X8BAIIND','Technic Matte Mineral Foundation - Café Au Lait',1100,'Piece',0,'','5021769125891','','http://202.166.166.100/oerp/my-assets/image/product/cafe_au_lait.jpg',1),('73227748','9FVJHH7X8BAIIND','Technic Matte Mineral Foundation - Sand',1100,'Piece',0,'','5021769458258','','http://202.166.166.100/oerp/my-assets/image/product/ty_(3).jpg',1),('56235236','9FVJHH7X8BAIIND','Technic Matte Mineral Foundation - Sorrel',1100,'Piece',0,'','5021769425182','','http://202.166.166.100/oerp/my-assets/image/product/Sorrel.jpg',1),('69631976','9FVJHH7X8BAIIND','Technic Matte Mineral Foundation - Cinnamon',1100,'Piece',0,'','5021769125877','','http://202.166.166.100/oerp/my-assets/image/product/ty_(2).jpg',1),('88326126','W29QGM6LFGMNK84','Technic Nail Varnich - Princess  ',450,'Piece',0,'','5021769241089','','http://202.166.166.100/oerp/my-assets/image/product/cec3598fe21c6b3074ddb07a0f00effd.jpg',1),('37144252','9FVJHH7X8BAIIND','Technic Beauty Boost Foundation - Biscuit',1025,'Piece',0,'','5021769247012','','http://202.166.166.100/oerp/my-assets/image/product/biscuit.jpg',1),('26318697','9FVJHH7X8BAIIND','Technic Beauty Boost Foundation - Oatmeal',1025,'Piece',0,'','5021769421337','','http://202.166.166.100/oerp/my-assets/image/product/Oatmeal.jpg',1),('35581896','9FVJHH7X8BAIIND','Technic Foundation Stix - Cinnamon',819,'Piece',0,'','5021769277019','','http://202.166.166.100/oerp/my-assets/image/product/Cinnamon.jpg',1),('66714832','9FVJHH7X8BAIIND','Technic Colour Fix 2-in-1 Pressed Powder & Cream Foundation -Buff',950,'Piece',0,'','5021769247043','','http://202.166.166.100/oerp/my-assets/image/product/Buff.jpg',1),('52729175','9FVJHH7X8BAIIND','Technic Colour Fix 2-in-1 Pressed Powder & Cream Foundation - Oatmeal',950,'Piece',0,'','5021787975232','','http://202.166.166.100/oerp/my-assets/image/product/Oatmeal.jpg',1),('44975588','9FVJHH7X8BAIIND','Technic Colour Fix 2-in-1 Pressed Powder & Cream Foundation - Biscuit',950,'Piece',0,'','5021769212546','','http://202.166.166.100/oerp/my-assets/image/product/Biscuit_1.jpg',1),('11225548','9FVJHH7X8BAIIND','Technic Colour Fix 2-in-1 Pressed Powder & Cream Foundation - Ecru',950,'Piece',0,'','5021769251453','','http://202.166.166.100/oerp/my-assets/image/product/Ecru.jpg',1),('52353221','9FVJHH7X8BAIIND','Technic Colour Fix Full Coverage Foundation - Buff',990,'Piece',0,'','5021769257011','','http://202.166.166.100/oerp/my-assets/image/product/Buff.jpg',1),('29579574','9FVJHH7X8BAIIND','Technic Colour Fix Full Coverage Foundation - Café Au Lait',990,'Piece',0,'','5021745210368','','http://202.166.166.100/oerp/my-assets/image/product/Cafe_au_Lait.jpg',1),('58811849','9FVJHH7X8BAIIND','Technic Colour Fix Full Coverage Foundation - Sand',990,'Piece',0,'','5021789500241','','http://202.166.166.100/oerp/my-assets/image/product/Sand.jpg',1),('92252955','W29QGM6LFGMNK84','Technic Nail Varnish - The A Team ',450,'Piece',0,'','5021769271161','','http://202.166.166.100/oerp/my-assets/image/product/ac13714b5ec1c7c25e10ecd87248de3b.jpg',1),('65612294','9FVJHH7X8BAIIND','Technic Colour Fix Full Coverage Foundation - Cinnamon',990,'Piece',0,'','5021789974127','','http://202.166.166.100/oerp/my-assets/image/product/Cinnamon.jpg',1),('25538539','9FVJHH7X8BAIIND','Technic Colour Fix Full Coverage Foundation - Tera Cota',990,'Piece',0,'','5021752224464','','http://202.166.166.100/oerp/my-assets/image/product/Terracota.jpg',1),('13973482','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Tropical',699,'Piece',0,'','5021775489758','','http://202.166.166.100/oerp/my-assets/image/product/c41b1d9a70156fbbbb07d241c3672545.jpg',1),('46568894','9FVJHH7X8BAIIND','Technic Conceal & Blend - Dark',699,'Piece',0,'','5021769541226','','http://202.166.166.100/oerp/my-assets/image/product/Dark.jpg',1),('34166762','9FVJHH7X8BAIIND','Technic Conceal & Blend - Medium',699,'Piece',0,'','5021769679448','','http://202.166.166.100/oerp/my-assets/image/product/Medium.jpg',1),('38462329','P7CFSTEKJ8KXMG9','Technic Exfoliating Lip Scrub',699,'Piece',0,'','5021769276210','','http://202.166.166.100/oerp/my-assets/image/product/7b2ced452a99e01b3475042799f60433.jpg',1),('44779861','9FVJHH7X8BAIIND','Technic Colour Fix Corrector Primer Purple',1349,'Piece',0,'','5021769267379','','http://202.166.166.100/oerp/my-assets/image/product/combats_dull_skin.jpg',1),('12342251','9FVJHH7X8BAIIND','Technic Anit Blemish Primer Pink',1349,'Piece',0,'','5021769267386','','http://202.166.166.100/oerp/my-assets/image/product/anti_blemish.jpg',1),('69386854','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Blood Orange',549,'Piece',0,'','5021769246091','','http://202.166.166.100/oerp/my-assets/image/product/7a5e71ea1b0e665061c028981b73ace5.jpg',1),('62359995','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Sing It Back',549,'Piece',0,'','5021769256139','','http://202.166.166.100/oerp/my-assets/image/product/07ec0e9eac86cb3173bcb94199dcf0cd.jpg',1),('56527628','9FVJHH7X8BAIIND','Technic Shade Adjusting Drops -Light',1199,'Piece',0,'','5021769277309','','http://202.166.166.100/oerp/my-assets/image/product/78269.jpg',1),('19165852','9FVJHH7X8BAIIND','Technic Strobe Kit - Blush',999,'Piece',0,'','5021769267102','','http://202.166.166.100/oerp/my-assets/image/product/14_(2).jpg',1),('46436494','9FVJHH7X8BAIIND','Technic Soft Focus Transparent Loose Powder',1399,'Piece',0,'','5021769257042','','http://202.166.166.100/oerp/my-assets/image/product/9_(1).jpg',1),('47142196','9FVJHH7X8BAIIND','Technic Healer Concealer -Light',599,'Piece',0,'','5021769277095','','http://202.166.166.100/oerp/my-assets/image/product/3_(1).jpg',1),('81468687','79Q3KB7P9P832FS','Technic Eyeliner & Smudger - Black',249,'Piece',0,'','5021769255017','','http://202.166.166.100/oerp/my-assets/image/product/90394c17948ec5903b9a0072e66d922b.jpg',1),('98655172','79Q3KB7P9P832FS','Technic Gel Eyeliner',649,'Piece',0,'','5021769215141','','http://202.166.166.100/oerp/my-assets/image/product/4cdd8ec2400321910837f97eb3e49704.jpg',1),('31441889','79Q3KB7P9P832FS','Technic Ultimate Brow Kit',1199,'Piece',0,'','5021769255086','','http://202.166.166.100/oerp/my-assets/image/product/1.jpg',1),('71592838','W29QGM6LFGMNK84','Technic Nail Varnich - Flamingo ',450,'Piece',0,'','5021769231561','','http://202.166.166.100/oerp/my-assets/image/product/8584e1173016d253b93d06687f8858b9.jpg',1),('43571975','79Q3KB7P9P832FS','Technic Eyebrow Pencil w/Sharpener and Brush - Black',249,'Piece',0,'','5021765412216','','http://202.166.166.100/oerp/my-assets/image/product/black.jpg',1),('61458585','79Q3KB7P9P832FS','Technic Eyebrow Pencil w/Sharpener and Brush - Brown Black',249,'Piece',0,'','5021722100088','','http://202.166.166.100/oerp/my-assets/image/product/black_brown.jpg',1),('38766572','79Q3KB7P9P832FS','Technic Mega Lash Mascara - Black',619,'Piece',0,'','5021769265122','','http://202.166.166.100/oerp/my-assets/image/product/00863c9d5b7b174eb47d6114c6f0a248.jpg',1),('73611942','79Q3KB7P9P832FS','Technic Lash Primer',699,'Piece',0,'','5021769285151','','http://202.166.166.100/oerp/my-assets/image/product/8e20a9c1addb880ab0f314fd8c60e5e0.jpg',1),('52328389','79Q3KB7P9P832FS','Technic Skinny Liquid Liner - Black',749,'Piece',0,'','5021769265139','','http://202.166.166.100/oerp/my-assets/image/product/rt_(2).jpg',1),('17189454','79Q3KB7P9P832FS','Technic Chunky Liquid Liner - Black',799,'Piece',0,'','5021769265146','','http://202.166.166.100/oerp/my-assets/image/product/b.jpg',1),('76817172','79Q3KB7P9P832FS','Technic Glitter Eyeliner - Blue',599,'Piece',0,'','5021769225065','','http://202.166.166.100/oerp/my-assets/image/product/blue.jpg',1),('47491915','P7CFSTEKJ8KXMG9','Technic Lip Liner Pencil - Nude',299,'Piece',0,'','5021785422257','','http://202.166.166.100/oerp/my-assets/image/product/a6820d6a1c5ab3413653431416128a9a.jpg',1),('75513118','79Q3KB7P9P832FS','Technic Glitter Eyeliner - Carnival',599,'Piece',0,'','5021769754213','','http://202.166.166.100/oerp/my-assets/image/product/carnival.jpg',1),('43891483','79Q3KB7P9P832FS','Technic Liquid Liner - Water Resistant',599,'Piece',0,'','5021769275060','','http://202.166.166.100/oerp/my-assets/image/product/display.jpg',1),('12495721','79Q3KB7P9P832FS','Technic Mega Lash Mascara - Water Resistant',649,'Piece',0,'','5021769275169','','http://202.166.166.100/oerp/my-assets/image/product/s-l500.jpg',1),('51943327','9FVJHH7X8BAIIND','Technic Matte Mineral Foundation - Tera Cota',1100,'Piece',0,'','5021769215479','','http://202.166.166.100/oerp/my-assets/image/product/69ea6d5f20ac42807567a97d7422e7b6.jpg',1),('47575149','79Q3KB7P9P832FS','Technic Lashitude Mascara - Black',699,'Piece',0,'','5021769245032','','http://202.166.166.100/oerp/my-assets/image/product/6_(2).jpg',1),('59621483','79Q3KB7P9P832FS','Technic Eyeshadow Primer - Shimmer',599,'Piece',0,'','5021769265160','','http://202.166.166.100/oerp/my-assets/image/product/technic-eyeshadow-primer-shimmerMM-8ml-p8182-10582_image.jpg',1),('17897237','79Q3KB7P9P832FS','Technic Get Lashed Mascara - Black',549,'Piece',0,'','5021769205036','','http://202.166.166.100/oerp/my-assets/image/product/ASD.jpg',1),('47249865','9FVJHH7X8BAIIND','Technic Conceal & Blend - Light',699,'Piece',0,'','5021769267065','','http://202.166.166.100/oerp/my-assets/image/product/f53b83dc9f3317069ef95bb517bf625d.jpg',1),('16495644','79Q3KB7P9P832FS','Technic Mega Lash Mascara with Argan Oil',699,'Piece',0,'','5021769285144','','http://202.166.166.100/oerp/my-assets/image/product/10_(3).jpg',1),('63319766','9FVJHH7X8BAIIND','Technic Colour Fix Full Coverage Foundation - Sorrel',990,'Piece',0,'','5021752003694','','http://202.166.166.100/oerp/my-assets/image/product/8d8df2ec90699491f113f47cf16b3502.jpg',1),('27138342','79Q3KB7P9P832FS','Technic Mega Nudes Eyeshadows Pallete',1099,'Piece',0,'','5021769245025','','http://202.166.166.100/oerp/my-assets/image/product/Mega_Nudes.jpg',1),('35651237','79Q3KB7P9P832FS','Technic Mega Nudes 3 Eyeshadows Pallete',1099,'Piece',0,'','5021769255093','','http://202.166.166.100/oerp/my-assets/image/product/Mega_Nudes_3.jpg',1),('42125549','79Q3KB7P9P832FS','Technic Mega Mattes Eyeshadows Pallete',1099,'Piece',0,'','5021769255130','','http://202.166.166.100/oerp/my-assets/image/product/Mega_Mattes.jpg',1),('64357596','79Q3KB7P9P832FS','Technic Mega Sultry Eyeshadows Pallete',1099,'Piece',0,'','5021769255109','','http://202.166.166.100/oerp/my-assets/image/product/Mega_Sultry.jpg',1),('77288629','79Q3KB7P9P832FS','Technic Eyelash Glue',499,'Piece',0,'','5021769215042','','http://202.166.166.100/oerp/my-assets/image/product/s-l1600.jpg',1),('66937463','79Q3KB7P9P832FS','Technic Natural False Eyelashes - A27',449,'Piece',0,'','5021769215165','','http://202.166.166.100/oerp/my-assets/image/product/A27.jpg',1),('31973839','79Q3KB7P9P832FS','Technic Natural False Eyelashes - BC21',449,'Piece',0,'','5021769255161','','http://202.166.166.100/oerp/my-assets/image/product/BC21.jpg',1),('52812878','79Q3KB7P9P832FS','Technic Natural False Eyelashes - BC14',449,'Piece',0,'','5021769255185','','http://202.166.166.100/oerp/my-assets/image/product/BC14.jpg',1),('72933825','79Q3KB7P9P832FS','Technic Natural False Eyelashes - A13',449,'Piece',0,'','5021769215158','','http://202.166.166.100/oerp/my-assets/image/product/A13.jpg',1),('46861688','79Q3KB7P9P832FS','Technic Natural False Eyelashes - A36',449,'Piece',0,'','5021769215172','','http://202.166.166.100/oerp/my-assets/image/product/A36.jpg',1),('68855634','79Q3KB7P9P832FS','Technic Glitter Eyeliner - Bronze',599,'Piece',0,'','5021752135647','','http://202.166.166.100/oerp/my-assets/image/product/5364d1a8b65548c17d045e29bff1e787.jpg',1),('76226952','79Q3KB7P9P832FS','Technic Natural False Eyelashes - BC31',449,'Piece',0,'','5021769255154','','http://202.166.166.100/oerp/my-assets/image/product/BC31.jpg',1),('67699587','79Q3KB7P9P832FS','Technic Natural False Eyelashes - BC19',449,'Piece',0,'','5021769255178','','http://202.166.166.100/oerp/my-assets/image/product/BC19.jpg',1),('27623275','P7CFSTEKJ8KXMG9','Technic Ombre Lip Pencil - Nude',699,'Piece',0,'','5021769276067','','http://202.166.166.100/oerp/my-assets/image/product/Nude.jpg',1),('68197248','P7CFSTEKJ8KXMG9','Technic Ombre Lip Pencil - Rose',699,'Piece',0,'','5021756231598','','http://202.166.166.100/oerp/my-assets/image/product/Rose.jpg',1),('61922372','P7CFSTEKJ8KXMG9','Technic Ombre Lip Pencil - Red',699,'Piece',0,'','5021752111320','','http://202.166.166.100/oerp/my-assets/image/product/Red.jpg',1),('63172769','P7CFSTEKJ8KXMG9','Technic Ombre Lip Pencil - Plum',699,'Piece',0,'','5021754896546','','http://202.166.166.100/oerp/my-assets/image/product/Plum.jpg',1),('34566946','P7CFSTEKJ8KXMG9','Technic Lip Liner Pencil - Coral',299,'Piece',0,'','5021769256045','','http://202.166.166.100/oerp/my-assets/image/product/coral.jpg',1),('57898978','9FVJHH7X8BAIIND','Technic Primer Spray',799,'Piece',0,'','5021769267164','','http://202.166.166.100/oerp/my-assets/image/product/e651d0844c05ad01666549cca869d16a.jpg',1),('32984827','9FVJHH7X8BAIIND','Technic Healer Concealer -Medium',599,'Piece',0,'','5021769277101','','http://202.166.166.100/oerp/my-assets/image/product/29caec4b918fc8f25ae6cafcfc75a304.jpg',1),('51242523','79Q3KB7P9P832FS','Technic Eyebrow Pencil w/Sharpener and Brush - Brown',249,'Piece',0,'','5021769255024','','http://202.166.166.100/oerp/my-assets/image/product/brown.jpg',1),('76177618','P7CFSTEKJ8KXMG9','Technic Lip Liner Pencil - Bright Red',299,'Piece',0,'','5021784562121','','http://202.166.166.100/oerp/my-assets/image/product/bright_red.jpg',1),('27343625','P7CFSTEKJ8KXMG9','Technic Lip Liner Pencil - Dark Red',299,'Piece',0,'','5021765945219','','http://202.166.166.100/oerp/my-assets/image/product/dark_red.jpg',1),('76447267','P7CFSTEKJ8KXMG9','Technic Liquid Lipstick - Case of Ex',799,'Piece',0,'','5021769266013','','http://202.166.166.100/oerp/my-assets/image/product/case-of-the-ex-Dark-Red-1024x1024.jpg',1),('16218137','P7CFSTEKJ8KXMG9','Technic Liquid Lipstick - Crave',799,'Piece',0,'','5021751123690','','http://202.166.166.100/oerp/my-assets/image/product/Crave-Dark-Pink-1024x1024.jpg',1),('96118932','9FVJHH7X8BAIIND','Technic Colour Fix Cream Foundation Contouring Palette',1450,'Piece',0,'','8662200333337 - T','','http://202.166.166.100/oerp/my-assets/image/product/aea20b644473ffc9a349cec5a91938f7.jpg',1),('63389971','P7CFSTEKJ8KXMG9','Technic Liquid Lipstick - Red Russian',799,'Piece',0,'','5021752879459','','http://202.166.166.100/oerp/my-assets/image/product/Red-Russian-Bright-Red-1024x1024.jpg',1),('48286412','P7CFSTEKJ8KXMG9','Technic Liquid Lipstick - Chat up',799,'Piece',0,'','5021796548779','','http://202.166.166.100/oerp/my-assets/image/product/Chat-Up-Nude-1024x1024.jpg',1),('44488722','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Smooch',699,'Piece',0,'','5021769120582','','http://202.166.166.100/oerp/my-assets/image/product/3_SMOOCH_12487.JPG',1),('26227319','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Honey Pie',699,'Piece',0,'','5021769414131','','http://202.166.166.100/oerp/my-assets/image/product/4_HONEY_PIE.jpg',1),('57184611','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Whirlwind',699,'Piece',0,'','5021769587453','','http://202.166.166.100/oerp/my-assets/image/product/2_whirlwind_____.jpg',1),('66741529','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Candy Kisses',699,'Piece',0,'','5021769678953','','http://202.166.166.100/oerp/my-assets/image/product/1_CANDY_KISSES.jpg',1),('62986341','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Tiny Dancer',699,'Piece',0,'','5021769121312','','http://202.166.166.100/oerp/my-assets/image/product/5_TINY_DANCER.jpg',1),('52235643','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Rebel',699,'Piece',0,'','5021769555551','','http://202.166.166.100/oerp/my-assets/image/product/6_REBEL12345.jpg',1),('66616128','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Naked',699,'Piece',0,'','5021769060765','','http://202.166.166.100/oerp/my-assets/image/product/11_NAKED.jpg',1),('56347176','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Clypso',699,'Piece',0,'','5021736316390','','http://202.166.166.100/oerp/my-assets/image/product/7_CALYPSO.jpg',1),('49649868','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Plum',699,'Piece',0,'','5021777745265','','http://202.166.166.100/oerp/my-assets/image/product/10_PLUM.jpg',1),('31845465','P7CFSTEKJ8KXMG9','Technic Hi Gloss Lipgloss - Crave',699,'Piece',0,'','5021769276173','','http://202.166.166.100/oerp/my-assets/image/product/4825e14392c17150d3d4ecbbe791ebe0.jpg',1),('14312268','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Watermelon',699,'Piece',0,'','5021787478962','','http://202.166.166.100/oerp/my-assets/image/product/12_WATERMELON.jpg',1),('23895138','W29QGM6LFGMNK84','Technic Nail Varnish - Bling-A-Ling',450,'Piece',0,'','5021769261087','','http://202.166.166.100/oerp/my-assets/image/product/db033f10f8ebcff5c5c5fc2a7a09f3ec.jpg',1),('84255148','W29QGM6LFGMNK84','Technic Nail Varnish - Violet  ',450,'Piece',0,'','5021769241126','','http://202.166.166.100/oerp/my-assets/image/product/5ae0eea82d903e70594b2e219917b3f3.jpg',1),('14487654','P7CFSTEKJ8KXMG9','Technic Lip Primer',699,'Piece',0,'','5021769286059','','http://202.166.166.100/oerp/my-assets/image/product/7_(1).jpg',1),('79628194','W29QGM6LFGMNK84','Technic Nail Varnish - Fairy Dust ',450,'Piece',0,'','5021769241195','','http://202.166.166.100/oerp/my-assets/image/product/dee39c863308aea9e06ef8b5a4cd6a06.jpg',1),('24475238','P7CFSTEKJ8KXMG9','Technic Hi Gloss Lipgloss - Mars',699,'Piece',0,'','5021769276180','','http://202.166.166.100/oerp/my-assets/image/product/4_(3).jpg',1),('42444451','P7CFSTEKJ8KXMG9','Technic Hi Gloss Lipgloss - Potion',699,'Piece',0,'','5021769276197','','http://202.166.166.100/oerp/my-assets/image/product/4_(1).jpg',1),('19297787','P7CFSTEKJ8KXMG9','Technic Hi Gloss Lipgloss - Kiss And Tell',699,'Piece',0,'','5021769276203','','http://202.166.166.100/oerp/my-assets/image/product/4_(4).jpg',1),('76152683','9FVJHH7X8BAIIND','Technic Colour Fix Corrector Primer Green',1349,'Piece',0,'','5021769267362','','http://202.166.166.100/oerp/my-assets/image/product/a5e0015197e55863967440216c00b865.jpg',1),('85412238','9FVJHH7X8BAIIND','Technic Setting Spray',799,'Piece',0,'','5021769267157','','http://202.166.166.100/oerp/my-assets/image/product/04c4d8c0b16a50256884c5e806cf39a7.jpg',1),('64611434','P7CFSTEKJ8KXMG9','Technic Pro Lipstick - Heartache #3',1199,'Piece',0,'','5021769276166','','http://202.166.166.100/oerp/my-assets/image/product/3_(2).jpg',1),('27711386','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick - Pink',599,'Piece',0,'','5021769246077','','http://202.166.166.100/oerp/my-assets/image/product/pink.jpg',1),('71513899','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick - Red',599,'Piece',0,'','5021765645218','','http://202.166.166.100/oerp/my-assets/image/product/red.jpg',1),('43362818','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick - Deep Red',599,'Piece',0,'','5021766632149','','http://202.166.166.100/oerp/my-assets/image/product/deep_red.jpg',1),('56294993','W29QGM6LFGMNK84','Technic Nail Varnich - spice World  ',450,'Piece',0,'','5021769271253','','http://202.166.166.100/oerp/my-assets/image/product/62f8a4f17c3bd8ea5cdb2715d2007215.jpg',1),('29684862','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick - Deep Purple',599,'Piece',0,'','5021765492157','','http://202.166.166.100/oerp/my-assets/image/product/deep_purple.jpg',1),('23573952','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick - Nude',599,'Piece',0,'','5021789721455','','http://202.166.166.100/oerp/my-assets/image/product/nude.jpg',1),('33776845','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Pink',599,'Piece',0,'','5021769256069','','http://202.166.166.100/oerp/my-assets/image/product/9_MATTE_PINK.jpeg',1),('21826863','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Red',599,'Piece',0,'','5021752146254','','http://202.166.166.100/oerp/my-assets/image/product/10_MATTE_RED.jpeg',1),('94241385','W29QGM6LFGMNK84','Technic Nail Varnich - Pop Art',450,'Piece',0,'','5021769271291','','http://202.166.166.100/oerp/my-assets/image/product/53c5fb6f7b2deb54094bb299467d5a30.jpg',1),('59648166','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Coral',599,'Piece',0,'','5021725665478','','http://202.166.166.100/oerp/my-assets/image/product/11_MATTE_CORAL.jpeg',1),('67365729','9FVJHH7X8BAIIND','Technic Corrector Stix ',880,'Piece',0,'','8662200222228 - T','','http://202.166.166.100/oerp/my-assets/image/product/6e05fa6a943af134418e11552c720e46.jpg',1),('41211877','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Nude',599,'Piece',0,'','5021798211367','','http://202.166.166.100/oerp/my-assets/image/product/12_MATTE_NUDE.jpeg',1),('45855167','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Be My Baby',599,'Piece',0,'','5021769266105','','http://202.166.166.100/oerp/my-assets/image/product/6_BE_MY_BABY_6.jpeg',1),('66953714','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Rumour Has It',599,'Piece',0,'','5021769266112','','http://202.166.166.100/oerp/my-assets/image/product/4_rumour_has_it_4.jpeg',1),('18968634','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Kiss Catch',599,'Piece',0,'','5021769266129','','http://202.166.166.100/oerp/my-assets/image/product/1_2.jpg',1),('29197187','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Dream Lover',599,'Piece',0,'','5021769266136','','http://202.166.166.100/oerp/my-assets/image/product/2_DREAM_LOVER_2.jpeg',1),('69883829','W29QGM6LFGMNK84','Technic Nail Varnish - Ice Cream Ella ',450,'Piece',0,'','5021769241027','','http://202.166.166.100/oerp/my-assets/image/product/188dc3b6c46362615b39e0fbc6f46ffd.jpg',1),('42476663','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Love Yourself',599,'Piece',0,'','5021769266150','','http://202.166.166.100/oerp/my-assets/image/product/3_LOVE_YOURSELF.jpg',1),('76156417','P7CFSTEKJ8KXMG9','Technic Viatmin E Lipstick - Hot Pink',549,'Piece',0,'','5021769296041','','http://202.166.166.100/oerp/my-assets/image/product/70c76dcfe4fda3239acaba3025800cc3.jpg',1),('15291467','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Hippy Dippy',549,'Piece',0,'','5021769256144','','http://202.166.166.100/oerp/my-assets/image/product/de99dc7cba626bfe4568e2840ffe977a.jpg',1),('53331771','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Bare',549,'Piece',0,'','5021769246138','','http://202.166.166.100/oerp/my-assets/image/product/bare-750x750.jpg',1),('35516844','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Pink Lady',549,'Piece',0,'','5021769296010','','http://202.166.166.100/oerp/my-assets/image/product/pinklady-750x750.jpg',1),('55962451','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick- Fuchsia Rose ',549,'Piece',0,'','5021769296034','','http://202.166.166.100/oerp/my-assets/image/product/fuchsia-rose750x750.jpg',1),('16174938','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Heartbeat',549,'Piece',0,'','5021769246084','','http://202.166.166.100/oerp/my-assets/image/product/heartbeat-750x750.jpg',1),('43484992','W29QGM6LFGMNK84','Technic Back Combing Brush  ',425,'Piece',0,'','5021769253013','','http://202.166.166.100/oerp/my-assets/image/product/085ee592ae9df9cb92d3602b5ff929b2.jpg',1),('75767819','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Ox Blood',699,'Piece',0,'','5021778444136','','http://202.166.166.100/oerp/my-assets/image/product/485c52d9b08a589ebc760dbdc569673a.jpg',1),('32281321','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Brief Encounter',549,'Piece',0,'','5021769276319','','http://202.166.166.100/oerp/my-assets/image/product/briefencounter-750x750.jpg',1),('22873153','P7CFSTEKJ8KXMG9','Technic Pro Lipstick - Miss Chief #2',1199,'Piece',0,'','5021769276159','','http://202.166.166.100/oerp/my-assets/image/product/61fbe7e2732210a67c3ecf59271f7d37.jpg',1),('55778466','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick - Coral',599,'Piece',0,'','5021755487965','','http://202.166.166.100/oerp/my-assets/image/product/1b1323dab9d563a80423cccfae419e04.jpg',1),('19778245','W29QGM6LFGMNK84','Technic Nail Varnish - Double Espresso ',450,'Piece',0,'','5021769271147','','http://202.166.166.100/oerp/my-assets/image/product/Double_expresso.jpg',1),('75731144','W29QGM6LFGMNK84','Technic Nail Varnish - Brooklyn Babe ',450,'Piece',0,'','5021769271154','','http://202.166.166.100/oerp/my-assets/image/product/27115_Brooklyn_Babe.jpg',1),('51533424','W29QGM6LFGMNK84','Technic Nail Varnish - Chocolatey Claire ',450,'Piece',0,'','5021769271178','','http://202.166.166.100/oerp/my-assets/image/product/27117_Chocolatey_Claire.jpg',1),('62123651','W29QGM6LFGMNK84','Technic Nail Varnish - Arabesque ',450,'Piece',0,'','5021769271192','','http://202.166.166.100/oerp/my-assets/image/product/27119_Arabesque.jpg',1),('65557469','W29QGM6LFGMNK84','Technic Nail Varnish - Hot Chilean ',450,'Piece',0,'','5021769271208','','http://202.166.166.100/oerp/my-assets/image/product/10_HOT_CHILEAN.jpg',1),('54214699','W29QGM6LFGMNK84','Technic Nail Varnish - All About Mimi',450,'Piece',0,'','5021769271215','','http://202.166.166.100/oerp/my-assets/image/product/27121_All_about_Mimi.jpg',1),('74718183','W29QGM6LFGMNK84','Technic Nail Varnish - Gold Member',450,'Piece',0,'','5021769271239','','http://202.166.166.100/oerp/my-assets/image/product/Gold_Member.jpg',1),('61659478','W29QGM6LFGMNK84','Technic Nail Varnish - Little Sis',450,'Piece',0,'','5021769271246','','http://202.166.166.100/oerp/my-assets/image/product/Little_Sis.jpg',1),('67833341','W29QGM6LFGMNK84','Technic Nail Varnich - Unicorn Tears',450,'Piece',0,'','5021769271260','','http://202.166.166.100/oerp/my-assets/image/product/Unicorn_Tears.jpg',1),('14217517','W29QGM6LFGMNK84','Technic Nail Varnish - Mermazing',450,'Piece',0,'','5021769271277','','http://202.166.166.100/oerp/my-assets/image/product/27127_Mermazing.jpg',1),('63765295','W29QGM6LFGMNK84','Technic Nail Varnish - Warhol',450,'Piece',0,'','5021769271284','','http://202.166.166.100/oerp/my-assets/image/product/10_WARHOL.jpg',1),('46124262','W29QGM6LFGMNK84','Technic Nail Varnish - Dance Off',450,'Piece',0,'','5021769261025','','http://202.166.166.100/oerp/my-assets/image/product/26102_Dance_Off.jpg',1),('17581624','W29QGM6LFGMNK84','Technic Nail Varnish - Festival Fever',450,'Piece',0,'','5021769261049','','http://202.166.166.100/oerp/my-assets/image/product/2199c3c18ddb635d39bcb20a2b7276fa.jpg',1),('51985917','W29QGM6LFGMNK84','Technic Nail Varnish - Negligee',450,'Piece',0,'','5021769261056','','http://202.166.166.100/oerp/my-assets/image/product/26105_Negligee.jpg',1),('54491587','W29QGM6LFGMNK84','Technic Nail Varnish - Pink Ladies',450,'Piece',0,'','5021769261063','','http://202.166.166.100/oerp/my-assets/image/product/26106_Pink_Ladies.jpg',1),('21853217','P7CFSTEKJ8KXMG9','Technic Lip Liner Pencil - Bright Pink',299,'Piece',0,'','5021788897458','','http://202.166.166.100/oerp/my-assets/image/product/8ac15e38961a0980dec420cd061684b2.jpg',1),('57294745','','Technic Liquid Lipstick - Date Night',799,'',0,'','5021756952110','','http://202.166.166.100/oerp/my-assets/image/product/d12a220a1f1b40e42fac8885b40fc1be.jpg',1),('34564525','W29QGM6LFGMNK84','Technic Nail Varnich - Pinky Swear',450,'Piece',0,'','5021769271307','','http://202.166.166.100/oerp/my-assets/image/product/10_PINKY_SWEAR.jpg',1),('35873834','W29QGM6LFGMNK84','Technic Nail Varnich - Full Moon Party',450,'Piece',0,'','5021769261001','','http://202.166.166.100/oerp/my-assets/image/product/10_FULL_MOON_PARTY.jpg',1),('26428783','W29QGM6LFGMNK84','Technic Nail Varnish - Boudoir',450,'Piece',0,'','5021769261117','','http://202.166.166.100/oerp/my-assets/image/product/26111_Boudoir.jpg',1),('72484215','W29QGM6LFGMNK84','Technic Nail Varnich - Aphrodite',450,'Piece',0,'','5021769261155','','http://202.166.166.100/oerp/my-assets/image/product/26115_Aphrodite.jpg',1),('55928712','W29QGM6LFGMNK84','Technic Nail Varnish - Ruby Red',450,'Piece',0,'','5021769261131','','http://202.166.166.100/oerp/my-assets/image/product/26113_Ruby_Red.jpg',1),('66796415','W29QGM6LFGMNK84','Technic Nail Varnich - Candyfloss',450,'Piece',0,'','5021769231134','','http://202.166.166.100/oerp/my-assets/image/product/10_CANDYFLOSSS.jpg',1),('86148819','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick- Bare All ',549,'Piece',0,'','5021769206019','','http://202.166.166.100/oerp/my-assets/image/product/1d945dfa4fec60190dc07382ef617f74.jpg',1),('42661445','W29QGM6LFGMNK84','Technic Nail Varnish - Black Velvet ',450,'Piece',0,'','5021769261162','','http://202.166.166.100/oerp/my-assets/image/product/Black_velvet.jpg',1),('77517419','W29QGM6LFGMNK84','Technic Nail Varnish - Guest List',450,'Piece',0,'','5021769261179','','http://202.166.166.100/oerp/my-assets/image/product/26117_Guest_List.jpg',1),('28384836','W29QGM6LFGMNK84','Technic Nail Varnish - Clear',450,'Piece',0,'','5021769271017','','http://202.166.166.100/oerp/my-assets/image/product/27101_Clear.jpg',1),('27694615','W29QGM6LFGMNK84','Technic Nail Varnish - White',450,'Piece',0,'','5021769211433','','http://202.166.166.100/oerp/my-assets/image/product/White.jpg',1),('19722853','9FVJHH7X8BAIIND','Technic Corrector and Contour Palette ',1250,'Piece',0,'','8662200222211 - T','','http://202.166.166.100/oerp/my-assets/image/product/6ad6002d4afa8732a59ad9ab2563bd2f.jpg',1),('29311953','W29QGM6LFGMNK84','Technic Nail Varnich - Lagoon',450,'Piece',0,'','5021769231592','','http://202.166.166.100/oerp/my-assets/image/product/10_LAGOON.jpg',1),('38998239','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Deep Purple',599,'Piece',0,'','5021769878988','','http://202.166.166.100/oerp/my-assets/image/product/179a5283b48d071636f93558dbf45571.jpeg',1),('14559614','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - OuiMadame!',599,'Piece',0,'','5021769266143','','http://202.166.166.100/oerp/my-assets/image/product/b2e9ed155f45009aae2975c6ebb89d87.jpeg',1),('66993765','P7CFSTEKJ8KXMG9','Technic Pro Lipstick - Cheerio Romeo #1',1199,'Piece',0,'','5021769276142','','http://202.166.166.100/oerp/my-assets/image/product/552fa678e834acd0ef72b1eb73091585.jpg',1),('65128598','W29QGM6LFGMNK84','Technic Nail Varnish - Seashell',450,'Piece',0,'','5021769231516','','http://202.166.166.100/oerp/my-assets/image/product/23151_Sea_Shell.jpg',1),('59682742','W29QGM6LFGMNK84','Technic Nail Varnich - Jet Black',450,'Piece',0,'','5021769271055','','http://202.166.166.100/oerp/my-assets/image/product/27105_Jet_Black.jpg',1),('64362819','W29QGM6LFGMNK84','Technic Nail Varnich - Tango Red',450,'Piece',0,'','5021769271123','','http://202.166.166.100/oerp/my-assets/image/product/27112_Tango_Red.jpg',1),('23483522','W29QGM6LFGMNK84','Technic Nail Varnich - Cactus Flower',450,'Piece',0,'','5021769271130','','http://202.166.166.100/oerp/my-assets/image/product/Cactus_Flower_1.jpg',1),('66216276','W29QGM6LFGMNK84','Technic Nail Varnich - Peony Pink',450,'Piece',0,'','5021769271109','','http://202.166.166.100/oerp/my-assets/image/product/peony_Pink.jpg',1),('15444644','W29QGM6LFGMNK84','Technic Nail Varnich - Pillar Box',450,'Piece',0,'','5021769241072','','http://202.166.166.100/oerp/my-assets/image/product/Pillar_Box.jpg',1),('62166484','9FVJHH7X8BAIIND','Technic Contour Stix ',880,'Piece',0,'','8662200111119 - T','','http://202.166.166.100/oerp/my-assets/image/product/aff5dcfde248c760eb7ec2a4982be8a2.jpg',1),('96482874','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Deep Red',599,'Piece',0,'','5021721321323','','http://202.166.166.100/oerp/my-assets/image/product/b60691118634ee15d56f40718c5749c6.jpeg',1),('96724851','9FVJHH7X8BAIIND','Technic Colour Fix Blush Palette  ',1450,'Piece',0,'','8662200222259 - T','','http://202.166.166.100/oerp/my-assets/image/product/8e3733cffd3f3a7ecde468fcc439d0c9.jpg',1),('16886114','9FVJHH7X8BAIIND','Technic Get Gorgeous Pink Sparkle Highlighter Powder ',850,'Piece',0,'','8662200222266 - T','','http://202.166.166.100/oerp/my-assets/image/product/487f65b601d845034dcfb8ed365ebf92.jpg',1),('86894214','9FVJHH7X8BAIIND','Technic Get Gorgeous 24CT Gold Highlighter Powder ',850,'Piece',0,'','8662200222853 - T','','http://202.166.166.100/oerp/my-assets/image/product/398ff8b85dbf411df16b2b72f9e48ff4.jpg',1),('17979528','9FVJHH7X8BAIIND','Technic Get Gorgeous highlighting Powder ',850,'Piece',0,'','8662200222860 - T','','http://202.166.166.100/oerp/my-assets/image/product/5eb9b1eb360e0f27b3ec6579affcd1ab.jpg',1),('54385866','9FVJHH7X8BAIIND','Technic Super Fine Matte Blusher ',750,'Piece',0,'','8662200222273 - T','','http://202.166.166.100/oerp/my-assets/image/product/766b12654502efe737497e086f4a24ba.jpg',1),('37897494','9FVJHH7X8BAIIND','Technic Highlights ',810,'Piece',0,'','8662200222280 - T','','http://202.166.166.100/oerp/my-assets/image/product/9699ed8fcb555e8cdb7ac935a1b49894.jpg',1),('39817635','9FVJHH7X8BAIIND','Technic Mega Blush ',810,'Piece',0,'','8662200222297 - T','','http://202.166.166.100/oerp/my-assets/image/product/128fc40d4f6b9935db0ca5bd35bb35ef.jpg',1),('31152412','9FVJHH7X8BAIIND','Technic Matte Mineral Foundation - Buff ',0,'Piece',0,'','8662200221252 - T','','http://202.166.166.100/oerp/my-assets/image/product/d4c10cabfe280d84d9fd151b2b9baf27.jpg',1),('73663383','9FVJHH7X8BAIIND','Technic Matte Mineral Foundation - Café u Lait ',0,'Piece',0,'','8662200221269 - T','','http://202.166.166.100/oerp/my-assets/image/product/b847e44f9e2b4736534410b50e0502fe.jpg',1),('89476856','9FVJHH7X8BAIIND','Technic Matte Mineral Foundation - Sand ',0,'Piece',0,'','8662200221276 - T','','http://202.166.166.100/oerp/my-assets/image/product/6c967b4df4576f2a4de9b7353dd06cac.jpg',1),('29861256','9FVJHH7X8BAIIND','Technic Matte Mineral Foundation - Sorrel ',0,'Piece',0,'','8662200221283 - T','','http://202.166.166.100/oerp/my-assets/image/product/6a95e202abe9e70ed7dece29faaf9365.jpg',1),('85357756','9FVJHH7X8BAIIND','Technic Matte Mineral Foundation - Cinammon ',0,'Piece',0,'','8662200221290 - T','','http://202.166.166.100/oerp/my-assets/image/product/2c95495503aba4837f3a1b407e9234e0.jpg',1),('89217173','9FVJHH7X8BAIIND','Technic Matte Mineral Foundation - Tera Cota ',0,'Piece',0,'','8662200221306 - T','','http://202.166.166.100/oerp/my-assets/image/product/01827f641bd914859737fcc035ac8c75.jpg',1),('26593387','9FVJHH7X8BAIIND','Technic Beauty Boost Foundation - Biscuit ',1025,'Piece',0,'','8662200222105 - T','','http://202.166.166.100/oerp/my-assets/image/product/b0c83a2b2b9f1568d6917505c59beea4.jpg',1),('14357447','9FVJHH7X8BAIIND','Technic Beauty Boost Foundation - Oatmeal ',1025,'Piece',0,'','8662200222877 - T','','http://202.166.166.100/oerp/my-assets/image/product/0b0c675eeaf4c9e652bbc6e20a4835d8.jpg',1),('38242799','9FVJHH7X8BAIIND','Technic Foundation Stix - Cinnamon ',819,'Piece',0,'','8662200502177 - T','','http://202.166.166.100/oerp/my-assets/image/product/75bff2f37d6daacf49d5e03bbd54ab71.jpg',1),('91716273','9FVJHH7X8BAIIND','Technic Colour Fix 2-in-1 Pressed Powder & Cream Foundation -Buff ',950,'Piece',0,'','8662200222204 - T','','http://202.166.166.100/oerp/my-assets/image/product/19c6eb107da09e8be37a4d0600fd1c23.jpg',1),('92163556','9FVJHH7X8BAIIND','Technic Colour Fix 2-in-1 Pressed Powder & Cream Foundation - Oatmeal ',950,'Piece',0,'','8662200222884 - T','','http://202.166.166.100/oerp/my-assets/image/product/6f49496dfeabc6d3b559a9d1cd1af261.jpg',1),('16697186','9FVJHH7X8BAIIND','Technic Colour Fix 2-in-1 Pressed Powder & Cream Foundation - Biscuit ',950,'Piece',0,'','8662200222891 - T','','http://202.166.166.100/oerp/my-assets/image/product/ea1997dcad42970d72ee3024a45b757c.jpg',1),('47944384','9FVJHH7X8BAIIND','Technic Colour Fix 2-in-1 Pressed Powder & Cream Foundation - Ecru ',950,'Piece',0,'','8662200222907 - T','','http://202.166.166.100/oerp/my-assets/image/product/897a1afb4e34775bd3d243b85cb82c9e.jpg',1),('95869884','9FVJHH7X8BAIIND','Technic Colour Fix Full Coverage Foundation - Buff ',990,'Piece',0,'','8662200222112 - T','','http://202.166.166.100/oerp/my-assets/image/product/dd09549549c8af46f3b291d0422e8e2d.jpg',1),('54488297','9FVJHH7X8BAIIND','Technic Colour Fix Full Coverage Foundation - Café u Lait ',990,'Piece',0,'','8662200222914 - T','','http://202.166.166.100/oerp/my-assets/image/product/b7be0d349f984139a7cc19991a392853.jpg',1),('77757352','9FVJHH7X8BAIIND','Technic Colour Fix Full Coverage Foundation - Sand ',990,'Piece',0,'','8662200222921 - T','','http://202.166.166.100/oerp/my-assets/image/product/5c358e645c8474484fa54ea6a482b402.jpg',1),('33237645','9FVJHH7X8BAIIND','Technic Colour Fix Full Coverage Foundation - Sorrel ',990,'Piece',0,'','8662200222938 - T','','http://202.166.166.100/oerp/my-assets/image/product/71a428c6fd6bdfb5ca5a42b7df621c02.jpg',1),('51828494','9FVJHH7X8BAIIND','Technic Colour Fix Full Coverage Foundation - Cinammon ',990,'Piece',0,'','8662200222945 - T','','http://202.166.166.100/oerp/my-assets/image/product/d036ae510dc25ef69d7816fd11467f9b.jpg',1),('71391422','9FVJHH7X8BAIIND','Technic Colour Fix Full Coverage Foundation - Tera Cota ',990,'Piece',0,'','8662200222952 - T','','http://202.166.166.100/oerp/my-assets/image/product/f2f4f32733f0f414fbc9850029516858.jpg',1),('81867689','9FVJHH7X8BAIIND','Technic Conceal & Blend - Light ',699,'Piece',0,'','8662200222129 - T','','http://202.166.166.100/oerp/my-assets/image/product/542a44cdabadbe3a642015ce27c554ac.jpg',1),('87552587','9FVJHH7X8BAIIND','Technic Conceal & Blend - Medium ',699,'Piece',0,'','8662200222969 - T','','http://202.166.166.100/oerp/my-assets/image/product/d3accb005e334719340c135be0b9fd22.jpg',1),('78122882','9FVJHH7X8BAIIND','Technic Conceal & Blend - Dark ',699,'Piece',0,'','8662200222976 - T','','http://202.166.166.100/oerp/my-assets/image/product/4999a5528537e6172cc6937b7850c3c5.jpg',1),('55182652','9FVJHH7X8BAIIND','Technic Colour Fix Corrector Primer Green ',1349,'Piece',0,'','8662200222136 - T','','http://202.166.166.100/oerp/my-assets/image/product/3b9ffe5ba1eb6797d5f922f4dd45d193.jpg',1),('72738337','9FVJHH7X8BAIIND','Technic Colour Fix Corrector Primer Purple',1349,'Piece',0,'','8662200222143','','http://202.166.166.100/oerp/my-assets/image/product/09530e63d3d1ab62d855e93834867628.jpg',1),('91176321','9FVJHH7X8BAIIND','Technic Anit Blemish Primer Pink',1349,'Piece',0,'','8662200222150','','http://202.166.166.100/oerp/my-assets/image/product/8ee79ae8f95b369a90eaf9835e161683.jpg',1),('95116862','9FVJHH7X8BAIIND','Technic Setting Spray ',799,'Piece',0,'','8662200222167 - T','','http://202.166.166.100/oerp/my-assets/image/product/7c90fcb1d60539e35975e4a819a05562.jpg',1),('45635275','9FVJHH7X8BAIIND','Technic Primer Spray ',799,'Piece',0,'','8662200222174 - T','','http://202.166.166.100/oerp/my-assets/image/product/2c8b825f29fcc2c51a3e61530cdebde9.jpg',1),('46454148','9FVJHH7X8BAIIND','Technic Shade Adjusting Drops -Light ',1199,'Piece',0,'','8662200222181 - T','','http://202.166.166.100/oerp/my-assets/image/product/13e815d0320b6f56a08d9c82e0e3c5ef.jpg',1),('45772378','9FVJHH7X8BAIIND','Technic Strobe Kit -Blush ',999,'Piece',0,'','8662200222198 - T','','http://202.166.166.100/oerp/my-assets/image/product/bc3a41becbc76e456d5296fa1b8e3a49.jpg',1),('78145559','9FVJHH7X8BAIIND','Technic Soft Focus Transparent Loose Powder ',1399,'Piece',0,'','8662200222235 - T','','http://202.166.166.100/oerp/my-assets/image/product/b5ab0a31d5cf708a51df3884331a89b9.jpg',1),('98228499','9FVJHH7X8BAIIND','Technic Healer Concealer -Light ',599,'Piece',0,'','8662200222242 - T','','http://202.166.166.100/oerp/my-assets/image/product/62ba5ca158bb89d4863139d8a4b6b71c.jpg',1),('32356294','9FVJHH7X8BAIIND','Technic Healer Concealer -Medium ',599,'Piece',0,'','8662200221313 - T','','http://202.166.166.100/oerp/my-assets/image/product/c2d6ab357e8667b5f31fd9f185514798.jpg',1),('54795555','79Q3KB7P9P832FS','Technic Ultimate Brow Kit ',1199,'Piece',0,'','8662200222303 - T','','http://202.166.166.100/oerp/my-assets/image/product/cf9ff1b2e4f9ece02b8c5331eacc90a1.jpg',1),('82128627','79Q3KB7P9P832FS','Technic Eyebrow Pencil w/Sharpener and Brush - Brown ',249,'Piece',0,'','8662200222310 - T','','http://202.166.166.100/oerp/my-assets/image/product/3579b15ad9861dbaeebbc6a5752a04b6.jpg',1),('13411811','79Q3KB7P9P832FS','Technic Eyebrow Pencil w/Sharpener and Brush - Black ',249,'Piece',0,'','8662200222983 - T','','http://202.166.166.100/oerp/my-assets/image/product/05829d46f75ec02e68fc2790116af03c.jpg',1),('49614498','79Q3KB7P9P832FS','Technic Eyebrow Pencil w/Sharpener and Brush - Brown Black ',249,'Piece',0,'','8662200222990 - T','','http://202.166.166.100/oerp/my-assets/image/product/d65325965d861ece983c8925d8b20980.jpg',1),('12826472','79Q3KB7P9P832FS','Technic Eyeliner & Smudger - Black',249,'Piece',0,'','8662200222327 - T','','http://202.166.166.100/oerp/my-assets/image/product/3af9cf3202dc8c5e0ecb91d9dafea1b1.jpg',1),('93729414','79Q3KB7P9P832FS','Technic Gel Eyeliner',649,'Piece',0,'','8662200222334 - T','','http://202.166.166.100/oerp/my-assets/image/product/ebf207a32ffcf57f09d8b3a98dc32a25.jpg',1),('58885451','79Q3KB7P9P832FS','Technic Skinny Liquid Liner ',749,'Piece',0,'','8662200222341 - T ','','http://202.166.166.100/oerp/my-assets/image/product/0f84aa533cd6789f2a292462c5667e45.jpg',1),('87572955','79Q3KB7P9P832FS','Technic Chunky Liquid Liner  ',799,'Piece',0,'','8662200222358 - T','','http://202.166.166.100/oerp/my-assets/image/product/c176304fc029c4bfea6ce71e02a56ed3.jpg',1),('92888129','79Q3KB7P9P832FS','Technic Glitter Eyeliner - Blue ',599,'Piece',0,'','8662200222365 - T','','http://202.166.166.100/oerp/my-assets/image/product/4e89f5f54fc77c632ff9ae0e76ba004c.jpg',1),('36518381','79Q3KB7P9P832FS','Technic Glitter Eyeliner - Bronze ',599,'Piece',0,'','8662200221009 - T','','http://202.166.166.100/oerp/my-assets/image/product/57de25c0bb07b374b25661a70d6b44d6.jpg',1),('63657454','79Q3KB7P9P832FS','Technic Glitter Eyeliner - Carnival  ',599,'Piece',0,'','8662200221016 - T','','http://202.166.166.100/oerp/my-assets/image/product/5523e0d26d2d1b2844ffa9d1d1515265.jpg',1),('32559217','79Q3KB7P9P832FS','Technic Liquid Liner - Water Resistant ',599,'Piece',0,'','8662200222372 - T','','http://202.166.166.100/oerp/my-assets/image/product/bf823b369de1c0c95ee4a149f76b9cde.jpg',1),('83847398','79Q3KB7P9P832FS','Technic Mega Lash Mascara - Water Resistant ',649,'Piece',0,'','8662200222389 - T','','http://202.166.166.100/oerp/my-assets/image/product/79be307cce968a176ebfac6d56597508.jpg',1),('32856757','79Q3KB7P9P832FS','Technic Mega Lash Mascara ',619,'Piece',0,'','8662200222396 - T','','http://202.166.166.100/oerp/my-assets/image/product/ff53e9da890e0419481a1b6f8c21fab1.jpg',1),('36826337','79Q3KB7P9P832FS','Technic Lashitude Mascara ',699,'Piece',0,'','8662200222402 - T','','http://202.166.166.100/oerp/my-assets/image/product/58a0e3a740bedc99692b8a2d90ea1ec3.jpg',1),('21563758','79Q3KB7P9P832FS','Technic Eyeshadow Primer - Shimmer ',599,'Piece',0,'','8662200222419 - T','','http://202.166.166.100/oerp/my-assets/image/product/bd1662944394d846c4094bdc26949b13.jpg',1),('83635849','79Q3KB7P9P832FS','Technic Get Lashed Mascara ',549,'Piece',0,'','8662200222426 - T','','http://202.166.166.100/oerp/my-assets/image/product/713605a147e2a75d9269633e899cbd75.jpg',1),('37194585','79Q3KB7P9P832FS','Technic Lash Primer ',699,'Piece',0,'','8662200222433 - T','','http://202.166.166.100/oerp/my-assets/image/product/7b341442edc7e0eded3e801d552e6540.jpg',1),('15649581','79Q3KB7P9P832FS','Technic Mega Lash with Argan Oil ',699,'Piece',0,'','8662200222440 - T','','http://202.166.166.100/oerp/my-assets/image/product/74c8572de190173ec94b456c34bd3a8b.jpg',1),('54854648','79Q3KB7P9P832FS','Technic Mega Nudes Eyeshadows ',1099,'Piece',0,'','8662200222457 - T','','http://202.166.166.100/oerp/my-assets/image/product/4e6c69dbbf9e6fffe7ac149efcffabca.jpg',1),('26247277','79Q3KB7P9P832FS','Technic Mega Nudes 3 Eyeshadows ',1099,'Piece',0,'','8662200222464 - T','','http://202.166.166.100/oerp/my-assets/image/product/a3ee3d83bf9bb970dd87f51ac6ce83ce.jpg',1),('85299512','79Q3KB7P9P832FS','Technic Mega Mattes Eyeshadows ',1099,'Piece',0,'','8662200222471 - T','','http://202.166.166.100/oerp/my-assets/image/product/30eb5d1e973b09f90fbdf3e4ab7ed95d.jpg',1),('58245217','79Q3KB7P9P832FS','Technic Mega Sultry Eyeshadows ',1099,'Piece',0,'','8662200222488 - T','','http://202.166.166.100/oerp/my-assets/image/product/46b28e7c4b31463969aa38a566a8f828.jpg',1),('73881177','9FVJHH7X8BAIIND','Technic Blusher Stick - Rosy cheeks ',799,'Piece',0,'','8662200502115 - T','','http://202.166.166.100/oerp/my-assets/image/product/906e0a70ab2d21104d3c1df9e6843ff1.jpg',1),('28544221','9FVJHH7X8BAIIND','Technic Blusher Stick - Flushed ',799,'Piece',0,'','8662200502191 - T','','http://202.166.166.100/oerp/my-assets/image/product/8945635b1e25671cf233aafd581697d8.jpg',1),('99785782','9FVJHH7X8BAIIND','Technic Blusher Stick - Peach Melba ',799,'Piece',0,'','8662200502108 - T','','http://202.166.166.100/oerp/my-assets/image/product/d5a20bfd61ee9b8824d962cf9348812f.jpg',1),('21315922','P7CFSTEKJ8KXMG9','Technic Ombre Lip Pencil - Nude ',699,'Piece',0,'','8662200222495 - T','','http://202.166.166.100/oerp/my-assets/image/product/a7ef64c32ab7efbc868032d03e6722ff.jpg',1),('39513595','P7CFSTEKJ8KXMG9','Technic Ombre Lip Pencil - Rose ',699,'Piece',0,'','8662200221023 - T','','http://202.166.166.100/oerp/my-assets/image/product/097ddc5415c681bcf4170b26ef579876.jpg',1),('88636935','P7CFSTEKJ8KXMG9','Technic Ombre Lip Pencil - Red ',699,'Piece',0,'','8662200221030 - T','','http://202.166.166.100/oerp/my-assets/image/product/514b69097ba7e98c5441b8adc5d61320.jpg',1),('95944555','P7CFSTEKJ8KXMG9','Technic Ombre Lip Pencil - Plum ',699,'Piece',0,'','8662200221047 - T','','http://202.166.166.100/oerp/my-assets/image/product/8cd8d8b1e7182124e2a8f451d578be3d.jpg',1),('78184648','P7CFSTEKJ8KXMG9','Technic Lip Liner Pencil - Coral ',299,'Piece',0,'','8662200222501 - T','','http://202.166.166.100/oerp/my-assets/image/product/134e8b1ea0085c35c56a5af7bbeb8e9e.jpg',1),('75642535','P7CFSTEKJ8KXMG9','Technic Lip Liner Pencil - Nude ',299,'Piece',0,'','8662200221054 - T','','http://202.166.166.100/oerp/my-assets/image/product/6a8aca6bfadd9812b8f22f320e5d45de.jpg',1),('86315763','P7CFSTEKJ8KXMG9','Technic Lip Liner Pencil - Bright Pink ',299,'Piece',0,'','8662200221061 - T','','http://202.166.166.100/oerp/my-assets/image/product/1d4d0b6032c50c4e06145a427617e310.jpg',1),('46821498','P7CFSTEKJ8KXMG9','Technic Lip Liner Pencil - Bright Red ',299,'Piece',0,'','8662200221078 - T','','http://202.166.166.100/oerp/my-assets/image/product/6bc64d03731334d475bb0340f753babb.jpg',1),('59333774','P7CFSTEKJ8KXMG9','Technic Lip Liner Pencil - Dark Red ',299,'Piece',0,'','8662200221085 - T','','http://202.166.166.100/oerp/my-assets/image/product/e63d42c96d440fe067aeec33ab8f88a1.jpg',1),('75169723','P7CFSTEKJ8KXMG9','Technic Liquid Lipstick - Case of Ex ',799,'Piece',0,'','8662200222518 - T','','http://202.166.166.100/oerp/my-assets/image/product/5f339dd6443bd83aa3fd4e070b9737f5.jpg',1),('41551268','P7CFSTEKJ8KXMG9','Technic Liquid Lipstick - Crave ',799,'Piece',0,'','8662200221092 - T','','http://202.166.166.100/oerp/my-assets/image/product/49b6ecc4916fc99fd70b3d02670bec76.jpg',1),('36365224','P7CFSTEKJ8KXMG9','Technic Liquid Lipstick - Date Night ',799,'Piece',0,'','8662200221108 - T','','http://202.166.166.100/oerp/my-assets/image/product/db2ad111e3c5d325c9094ed8f36179c7.jpg',1),('39942618','P7CFSTEKJ8KXMG9','Technic Liquid Lipstick - Red Russian ',799,'Piece',0,'','8662200221115 - T','','http://202.166.166.100/oerp/my-assets/image/product/c69ee4c7059f063f07577c57d1ff4758.jpg',1),('35978473','P7CFSTEKJ8KXMG9','Technic Liquid Lipstick - Chat up ',799,'Piece',0,'','8662200221122 - T','','http://202.166.166.100/oerp/my-assets/image/product/06eb6a41a8c7488f3761727082a93dbd.jpg',1),('19554935','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Smooch',699,'Piece',0,'','8662200222525 - T','','http://202.166.166.100/oerp/my-assets/image/product/6e1e2d72fca3cc3cb8186291f4d083e6.JPG',1),('23461782','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Honey Pie ',699,'Piece',0,'','8662200221153 - T','','http://202.166.166.100/oerp/my-assets/image/product/7eec62dded196348758fe85681fbee05.jpg',1),('34433392','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Whirlwind ',699,'Piece',0,'','8662200221177 - T','','http://202.166.166.100/oerp/my-assets/image/product/a92e25a8c1de96a0cb4372dd83153bbb.jpg',1),('78283483','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Candy Kisses ',699,'Piece',0,'','8662200221139 - T','','http://202.166.166.100/oerp/my-assets/image/product/c7a653464b11bf9e5b1fae7978570580.jpg',1),('53668145','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Tiny Dancer ',699,'Piece',0,'','8662200221160 - T','','http://202.166.166.100/oerp/my-assets/image/product/8813a817fb56372402e4db53670ad052.jpg',1),('53823878','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Rebel ',699,'Piece',0,'','8662200221146 - T','','http://202.166.166.100/oerp/my-assets/image/product/cdd78274a20130b31d73a4ed40ca71e4.jpg',1),('38743941','P7CFSTEKJ8KXMG9','Technic Exfoliating Lip Scrub ',699,'Piece',0,'','8662200222532 - T','','http://202.166.166.100/oerp/my-assets/image/product/85ada5d6785767ca9ccc582253cf0430.jpg',1),('47981972','P7CFSTEKJ8KXMG9','Technic Lip Primer ',699,'Piece',0,'','8662200222549 - T','','http://202.166.166.100/oerp/my-assets/image/product/52e84568e89af378a1c7cca0a446151c.jpg',1),('93232524','P7CFSTEKJ8KXMG9','Technic Hi Gloss Lipgloss - Crave ',699,'Piece',0,'','8662200221184 - T','','http://202.166.166.100/oerp/my-assets/image/product/6225c46d33ba01da5b5b2e685fd47f7e.jpg',1),('62789466','P7CFSTEKJ8KXMG9','Technic Hi Gloss Lipgloss - Mars ',699,'Piece',0,'','8662200221191 - T','','http://202.166.166.100/oerp/my-assets/image/product/feddf5a281a677645b0a70103f6b7f80.jpg',1),('19338288','P7CFSTEKJ8KXMG9','Technic Hi Gloss Lipgloss - Potion ',699,'Piece',0,'','8662200221214 - T','','http://202.166.166.100/oerp/my-assets/image/product/c8b1a2b2de768f8f0d54797ac753e14d.jpg',1),('53118748','P7CFSTEKJ8KXMG9','Technic Hi Gloss Lipgloss - Kiss And Tell ',699,'Piece',0,'','8662200221221 - T','','http://202.166.166.100/oerp/my-assets/image/product/1584b2b2ea694ff66b55ef918fab4c11.jpg',1),('17795987','P7CFSTEKJ8KXMG9','Technic Pro Lipstick - Cheerio Romeo #1 ',1199,'Piece',0,'','8662200222563 - T','','http://202.166.166.100/oerp/my-assets/image/product/99d7938be35a119f71ee5bf1bdf0a00d.jpg',1),('25441358','P7CFSTEKJ8KXMG9','Technic Pro Lipstick - Miss Chief #2',1199,'Piece',0,'','8662200221238 - T','','http://202.166.166.100/oerp/my-assets/image/product/30ac36e8e197599561c358800b2dd37c.jpg',1),('66492183','P7CFSTEKJ8KXMG9','Technic Pro Lipstick - Heartache #3 ',1199,'Piece',0,'','8662200221245 - T','','http://202.166.166.100/oerp/my-assets/image/product/559140c79b6f747af10e5f45397f828d.jpg',1),('12296577','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick - Pink ',599,'Piece',0,'','8662200222693 - T','','http://202.166.166.100/oerp/my-assets/image/product/b7166730cdf1af7c53e4f29158d88df2.jpg',1),('81685874','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick - Red ',599,'Piece',0,'','8662200222709 - T','','http://202.166.166.100/oerp/my-assets/image/product/f46b77ea2ee4e48947addbcf3f8721b3.jpg',1),('52192942','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick - Deep Red ',599,'Piece',0,'','8662200222716 - T','','http://202.166.166.100/oerp/my-assets/image/product/670737460152f2ff40c9a915fa115a46.jpg',1),('73316883','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick - Coral ',599,'Piece',0,'','8662200222723 - T','','http://202.166.166.100/oerp/my-assets/image/product/65766128a60cf1eb58fe109fdd4531c7.jpg',1),('48349392','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick - Deep Purple ',599,'Piece',0,'','8662200222730 - T','','http://202.166.166.100/oerp/my-assets/image/product/f5baff58fa82178fdfe8564012b13785.jpg',1),('35899631','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick - Nude ',599,'Piece',0,'','8662200222747 - T','','http://202.166.166.100/oerp/my-assets/image/product/b9eedd2eb49e5ca4ce8c62e97378134c.jpg',1),('64231489','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Pink ',599,'Piece',0,'','8662200222754 - T','','http://202.166.166.100/oerp/my-assets/image/product/39ffa07338321f7d803b62b0ce9c20f5.jpeg',1),('39966796','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Red',599,'Piece',0,'','8662200222761 - T','','http://202.166.166.100/oerp/my-assets/image/product/3d387c07b491abbef7bc98f3d72f43b5.jpeg',1),('75841999','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Deep Red ',599,'Piece',0,'','8662200222778 - T','','http://202.166.166.100/oerp/my-assets/image/product/fcdc038a3deba81e6de1f5e474568c6b.jpeg',1),('27787243','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Coral ',599,'Piece',0,'','8662200222785 - T','','http://202.166.166.100/oerp/my-assets/image/product/b2e08c44882eddd97316954b19e63842.jpeg',1),('43823574','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Deep Purple ',599,'Piece',0,'','8662200222792 - T','','http://202.166.166.100/oerp/my-assets/image/product/90c0ae448b1906a2665509f61a7f4ffc.jpeg',1),('35387537','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Nude ',599,'Piece',0,'','8662200222808 - T','','http://202.166.166.100/oerp/my-assets/image/product/be1ea6f11c6803d7ce01ad9582c1264c.jpeg',1),('51814898','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Be My Baby ',599,'Piece',0,'','8662200222815 - T','','http://202.166.166.100/oerp/my-assets/image/product/825135e13e1756390bdd9c068434a705.jpeg',1),('24432454','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Rumour Has It ',599,'Piece',0,'','8662200222822 - T','','http://202.166.166.100/oerp/my-assets/image/product/a223a90486345841b9257a58c9ea7246.jpeg',1),('59892815','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Kiss Catch ',599,'Piece',0,'','8662200222839 - T','','http://202.166.166.100/oerp/my-assets/image/product/e7a5d3071a0a97dc289f9796ede85aa2.jpg',1),('82991274','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Dream Lover ',599,'Piece',0,'','8662200222846 - T','','http://202.166.166.100/oerp/my-assets/image/product/958ce6c9aae5e6cc2d6378d51027269f.jpeg',1),('55426419','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - OuiMadame! ',599,'Piece',0,'','8662200222570 - T','','http://202.166.166.100/oerp/my-assets/image/product/4020546ff3c830bb291ff0ea0381c87f.jpeg',1),('72945797','P7CFSTEKJ8KXMG9','Technic Colour Max Lipstick Matte - Love Yourself ',599,'Piece',0,'','8662200222587 - T','','http://202.166.166.100/oerp/my-assets/image/product/f3b3fd163ee5d1a2a179a5d3c7fabcb3.jpg',1),('76394175','P7CFSTEKJ8KXMG9','Technic Viatmin E Lipstick - Hot Pink ',549,'Piece',0,'','8662200222594 - T','','http://202.166.166.100/oerp/my-assets/image/product/096e0558a5ecf71e1d7940bd02182f1c.jpg',1),('26956683','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Hippy Dippy ',549,'Piece',0,'','8662200222600 - T','','http://202.166.166.100/oerp/my-assets/image/product/be24a074d1e6d3a0aea6d520df67e525.jpg',1),('99243581','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Bare ',549,'Piece',0,'','8662200222617 - T','','http://202.166.166.100/oerp/my-assets/image/product/163ae7d2b926bce45a42a79a4df42c55.jpg',1),('11553738','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Pink Lady ',549,'Piece',0,'','8662200222624 - T','','http://202.166.166.100/oerp/my-assets/image/product/fd715684f2a2433eed5654e17b6ea218.jpg',1),('15444364','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick- Fuchsia Rose  ',549,'Piece',0,'','8662200222631 - T','','http://202.166.166.100/oerp/my-assets/image/product/2365ff68a406622d56af572a8f6c00da.jpg',1),('31371779','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Heartbeat ',549,'Piece',0,'','8662200222648 - T','','http://202.166.166.100/oerp/my-assets/image/product/9ef9d46e5684d99b9bc03c7b19318ed3.jpg',1),('66416536','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Blood Orange ',549,'Piece',0,'','8662200222655 - T','','http://202.166.166.100/oerp/my-assets/image/product/6eeaac34ef7eb47eade9da297b4ec329.jpg',1),('68876788','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Sing It Back ',549,'Piece',0,'','8662200222662 - T','','http://202.166.166.100/oerp/my-assets/image/product/611f9d61628824e45fe6dbd1a3a37653.jpg',1),('41767343','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick - Brief Encounter ',549,'Piece',0,'','8662200222679 - T','','http://202.166.166.100/oerp/my-assets/image/product/6bb8b699cd93b0947849009341b2f9fa.jpg',1),('44556474','P7CFSTEKJ8KXMG9','Technic Vitamin E Lipstick- Bare All  ',549,'Piece',0,'','8662200222686 - T','','http://202.166.166.100/oerp/my-assets/image/product/5a3c24016a0518b7e8b36308a7599ef0.jpg',1),('19351199','9FVJHH7X8BAIIND','Technic Blusher Stick - Rosy Cheeks ',799,'Piece',0,'','5021769257066','','http://202.166.166.100/oerp/my-assets/image/product/e82100c00d5785c70e9110f85c7eca6b.jpg',1),('14619457','9FVJHH7X8BAIIND','Technic Blusher Stick - Flushed ',799,'Piece',0,'','5021769277033','','http://202.166.166.100/oerp/my-assets/image/product/5439ed4b49597758246cc59970cb6a03.jpg',1),('12198562','9FVJHH7X8BAIIND','Technic Blusher Stick - Peach Melba ',799,'Piece',0,'','5021769277026','','http://202.166.166.100/oerp/my-assets/image/product/1e6449f2ec5c3507e4c535ea35320349.jpg',1),('87474976','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Naked',699,'Piece',0,'','8662200222525','','http://202.166.166.100/oerp/my-assets/image/product/8b00c978f641b7d905916635c0ed78ed.jpg',1),('36439921','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Clypso',699,'Piece',0,'','8662200221153','','http://202.166.166.100/oerp/my-assets/image/product/1fadc70cfbd4e4eb227eb15ed83fa438.jpg',1),('89424962','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Plum',699,'Piece',0,'','8662200221177','','http://202.166.166.100/oerp/my-assets/image/product/042e4f6e616632e58bbe44517b961533.jpg',1),('79488192','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Ox Blood',699,'Piece',0,'','8662200221139','','http://202.166.166.100/oerp/my-assets/image/product/4a151031d560b5ac08e6f6a90fece7b0.jpg',1),('13866375','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Watermelon',699,'Piece',0,'','8662200221160','','http://202.166.166.100/oerp/my-assets/image/product/f834b6502cbcaf4b6da95a0df8e9dd5d.jpg',1),('58648897','P7CFSTEKJ8KXMG9','Technic Juicy Stick - Tropical',699,'Piece',0,'','8662200221146','','http://202.166.166.100/oerp/my-assets/image/product/b41775aa47936d9f906b51cef357e75f.jpg',1);
/*!40000 ALTER TABLE `product_information` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_price_history`
--

DROP TABLE IF EXISTS `product_price_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `product_price_history` (
  `product_pr_his_id` int(11) NOT NULL,
  `product_id` varchar(30) CHARACTER SET latin1 NOT NULL,
  `supplier_id` varchar(30) CHARACTER SET latin1 NOT NULL,
  `supplier_price` double NOT NULL,
  `date_of_price_buy` varchar(30) CHARACTER SET latin1 NOT NULL,
  `affect_time_pc` varchar(30) CHARACTER SET latin1 NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_price_history`
--

LOCK TABLES `product_price_history` WRITE;
/*!40000 ALTER TABLE `product_price_history` DISABLE KEYS */;
/*!40000 ALTER TABLE `product_price_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_purchase`
--

DROP TABLE IF EXISTS `product_purchase`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `product_purchase` (
  `purchase_id` varchar(100) NOT NULL,
  `chalan_no` varchar(100) NOT NULL,
  `supplier_id` varchar(100) NOT NULL,
  `grand_total_amount` float NOT NULL,
  `total_discount` float DEFAULT NULL,
  `purchase_date` varchar(50) NOT NULL,
  `purchase_details` text NOT NULL,
  `status` int(2) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_purchase`
--

LOCK TABLES `product_purchase` WRITE;
/*!40000 ALTER TABLE `product_purchase` DISABLE KEYS */;
INSERT INTO `product_purchase` VALUES ('20200118145430','9870','HW78JZTO8ZD3ASA8VIYA',43500,NULL,'2020-01-18','LAHORE',1),('20200131195408','G10002','P85V2LTWR5XHC4G5ZRFS',14471600,NULL,'2019-10-01','',1),('20200124171415','0987','VNF1K363UQXDVU1SMF9E',43200,NULL,'2020-01-24','',1),('20200131184632','GT10001','P85V2LTWR5XHC4G5ZRFS',1654940,NULL,'2019-10-01','',1);
/*!40000 ALTER TABLE `product_purchase` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_purchase_details`
--

DROP TABLE IF EXISTS `product_purchase_details`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `product_purchase_details` (
  `purchase_detail_id` varchar(100) NOT NULL,
  `purchase_id` varchar(100) NOT NULL,
  `product_id` varchar(100) NOT NULL,
  `quantity` float NOT NULL,
  `rate` float NOT NULL,
  `total_amount` float NOT NULL,
  `discount` float DEFAULT NULL,
  `status` int(11) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_purchase_details`
--

LOCK TABLES `product_purchase_details` WRITE;
/*!40000 ALTER TABLE `product_purchase_details` DISABLE KEYS */;
INSERT INTO `product_purchase_details` VALUES ('VshGn4gtPvyeiBO','20200118145430','57317359',3,14500,43500,NULL,1),('lI7MAqGKZ28l1fC','20200131184632','79488192',10,489.3,4893,NULL,0),('SA0eF6nZXsVklfr','20200131184632','89424962',10,489.3,4893,NULL,0),('nOm3u3JsvxN0iaP','20200131184632','36439921',10,489.3,4893,NULL,0),('jtNF7IUcF6M3LH0','20200131184632','87474976',10,489.3,4893,NULL,0),('HmPgQR83neMvOPt','20200131184632','53823878',10,489.3,4893,NULL,0),('ddfN7Mf7luxQ5Gc','20200131184632','53668145',10,489.3,4893,NULL,0),('fxZoCblWgTHtC9T','20200131184632','78283483',10,489.3,4893,NULL,0),('eKscsUAYO2oJULN','20200131184632','34433392',10,489.3,4893,NULL,0),('lNJOC7VFtuNq1mb','20200131184632','23461782',10,489.3,4893,NULL,0),('iQ9ilhp62Q5chDL','20200131184632','19554935',10,489.3,4893,NULL,0),('I8n1xFDxaH7FZvo','20200131184632','35978473',30,559.3,16779,NULL,0),('fU0VjHVUk9lRJtD','20200131184632','39942618',15,559.3,8389.5,NULL,0),('MkrzbM0VDJpSyiI','20200131184632','36365224',15,559.3,8389.5,NULL,0),('v2zPH0ucoRrKIv0','20200131184632','41551268',10,559.3,5593,NULL,0),('X6BM3cL5ByqKNsN','20200124171415','57317359',3,14400,43200,NULL,1),('ysm2fJ31KVOp41q','20200131184632','75169723',30,559.3,16779,NULL,0),('BIdQKeVohOf2EyF','20200131184632','59333774',30,209.3,6279,NULL,0),('4hCwhBTAbXfilB3','20200131184632','46821498',30,209.3,6279,NULL,0),('HlPBBEdC52VzSMu','20200131184632','86315763',30,209.3,6279,NULL,0),('US3A9icaCeGMfUZ','20200131184632','75642535',30,209.3,6279,NULL,0),('Rz4ZycA1XS6b45k','20200131184632','78184648',30,209.3,6279,NULL,0),('YRrpqcbdBqLBMLl','20200131184632','95944555',30,489.3,14679,NULL,0),('oRH0ilB6RFDk9Ja','20200131184632','88636935',30,489.3,14679,NULL,0),('IRRYvwwjOs0g0kE','20200131184632','39513595',30,489.3,14679,NULL,0),('rDVQdCXpDIU4FzZ','20200131184632','21315922',30,489.3,14679,NULL,0),('b2PeeYyc6ZSRlho','20200131184632','28544221',0,559.3,0,NULL,0),('fqjZrGNAvn17ffD','20200131184632','58245217',30,769.3,23079,NULL,0),('WNCmIV8hlzTLPXG','20200131184632','85299512',30,769.3,23079,NULL,0),('Sc5BhwUobzL3pG6','20200131184632','26247277',30,769.3,23079,NULL,0),('GGQ7SVYQ3ykZC5Q','20200131184632','54854648',30,769.3,23079,NULL,0),('insE3RHoggbz5Dq','20200131184632','15649581',30,489.3,14679,NULL,0),('DjzLEb9haUhMV2Q','20200131184632','37194585',30,489.3,14679,NULL,0),('wbgPjCwOZwUYl2L','20200131184632','83635849',30,384.3,11529,NULL,0),('soIelwd9TAxBUDM','20200131184632','21563758',30,419.3,12579,NULL,0),('YScIghoWAF9yhxb','20200131184632','36826337',30,489.3,14679,NULL,0),('1qv8o9WihzpUPNc','20200131184632','32856757',30,433.3,12999,NULL,0),('FAtMEg1damycymK','20200131184632','83847398',30,454.3,13629,NULL,0),('EkG8MY4icHwagJI','20200131184632','32559217',30,419.3,12579,NULL,0),('2lHDxmZ4vVCNysC','20200131184632','63657454',30,419.3,12579,NULL,0),('3E4mS6TDOM8ZXsj','20200131184632','36518381',15,419.3,6289.5,NULL,0),('4iNLeTGJ5JadqzP','20200131184632','92888129',30,419.3,12579,NULL,0),('OntOCtvS34KJuKe','20200131184632','87572955',30,559.3,16779,NULL,0),('YsS6J5ojDRigYwN','20200131184632','58885451',30,524.3,15729,NULL,0),('2h0Y0hQJI3R20VI','20200131184632','93729414',30,454.3,13629,NULL,0),('XKhaltaIm0dHMpz','20200131184632','12826472',30,174.3,5229,NULL,0),('3Jgbh2KkmHtGDRZ','20200131184632','49614498',30,174.3,5229,NULL,0),('4ICbL0h4jTfMLro','20200131184632','13411811',30,174.3,5229,NULL,0),('c3jXVsCu8Ie6hSV','20200131184632','82128627',30,174.3,5229,NULL,0),('1shD7Grjxmn1Nbq','20200131184632','54795555',30,839.3,25179,NULL,0),('b46cIpZPFu8ZxYT','20200131184632','32356294',30,419.3,12579,NULL,0),('UNNOiomr5DCjxzP','20200131184632','98228499',30,419.3,12579,NULL,0),('DpBPt7GaVsUDb6C','20200131184632','78145559',30,979.3,29379,NULL,0),('RK6TbYJQbVotxqo','20200131184632','45772378',30,699.3,20979,NULL,0),('WND61qXm4ssl03','20200131184632','46454148',30,839.3,25179,NULL,0),('KCERcyyCLYScB6Q','20200131184632','45635275',30,559.3,16779,NULL,0),('b7EjonbvNVGsRJp','20200131184632','95116862',30,559.3,16779,NULL,0),('4ZGaugjWkPRmmx9','20200131184632','91176321',30,944.3,28329,NULL,0),('IGHri3Hj3Tf7D74','20200131184632','72738337',30,944.3,28329,NULL,0),('cNhTZJmDp5BXgTy','20200131184632','55182652',30,944.3,28329,NULL,0),('uSjcvZA0M7Esb8z','20200131184632','78122882',30,489.3,14679,NULL,0),('Pr4NmRofS8slVbm','20200131184632','87552587',30,489.3,14679,NULL,0),('27sc1hOx6Eee4HA','20200131184632','81867689',30,489.3,14679,NULL,0),('j386Yb4AUNUWsAa','20200131184632','71391422',20,693,13860,NULL,0),('7Sl4yv8Y1qiyngW','20200131184632','51828494',15,693,10395,NULL,0),('TkWgd8m3USCgrDI','20200131184632','33237645',15,693,10395,NULL,0),('Oc8gKDjEtK978h7','20200131184632','77757352',15,693,10395,NULL,0),('DZK4lGYTpJMZBmO','20200131184632','54488297',15,693,10395,NULL,0),('1sRrGC5ql4Nwj0Y','20200131184632','95869884',15,693,10395,NULL,0),('av1L7ftjv2t2YzM','20200131184632','47944384',20,665,13300,NULL,0),('4D2XvNji0gKpxmd','20200131184632','16697186',15,665,9975,NULL,0),('JWvKpimPIKGgFWb','20200131184632','92163556',20,665,13300,NULL,0),('rZdrVNxw7mqQbyn','20200131184632','91716273',20,665,13300,NULL,0),('iIBUwckjh1Y6P5x','20200131184632','14357447',30,717.5,21525,NULL,0),('fp06tFbtz4UzHWr','20200131184632','26593387',30,717.5,21525,NULL,0),('vVvoaQwFQzXZg3c','20200131184632','89217173',30,0,0,NULL,0),('xhaDFgAtcj0MD3a','20200131184632','85357756',30,0,0,NULL,0),('bV3GfIp0CRIrfD9','20200131184632','29861256',30,0,0,NULL,0),('ebQiG8n6iQvd3Ns','20200131184632','89476856',30,0,0,NULL,0),('IKoKPje4mIpM5RR','20200131184632','73663383',30,0,0,NULL,0),('J9EpqFzlmz6mbOj','20200131184632','31152412',30,0,0,NULL,0),('7FCE5UnDovkwmEj','20200131184632','39817635',30,567,17010,NULL,0),('5egsBJ5hAU0lU8r','20200131184632','37897494',30,567,17010,NULL,0),('ITIzRiIVHhI2dxg','20200131184632','54385866',30,525,15750,NULL,0),('r0ygnNg1Ybl08Fn','20200131184632','17979528',30,595,17850,NULL,0),('Peq7aUCjYyt7opa','20200131184632','86894214',30,595,17850,NULL,0),('VXzoYANN5ukgenK','20200131184632','16886114',30,595,17850,NULL,0),('2gr6mGIEvKh17eC','20200131184632','96724851',30,1015,30450,NULL,0),('4wVAKix5iYltCnu','20200131184632','62166484',30,616,18480,NULL,0),('47EGZMbpBdFw7LX','20200131184632','19722853',30,875,26250,NULL,0),('l7HXMxt0krLoBAy','20200131184632','67365729',30,616,18480,NULL,0),('6PMcJTPxBaucGl8','20200131184632','96118932',30,1015,30450,NULL,0),('FxDESIA7iXo39QX','20200131184632','13866375',10,489.3,4893,NULL,0),('q2fmFgULqsCnV4J','20200131184632','58648897',10,489.3,4893,NULL,0),('X08aDPjDLHb10ZG','20200131184632','38743941',25,489.3,12232.5,NULL,0),('XG94sDZCPGvSuFL','20200131184632','47981972',25,489.3,12232.5,NULL,0),('5cK24OHcSUjtJjJ','20200131184632','93232524',30,489.3,14679,NULL,0),('Q7PPz9UPAiTv1G2','20200131184632','62789466',30,489.3,14679,NULL,0),('CAVM3Mb0ptSPdcy','20200131184632','19338288',30,489.3,14679,NULL,0),('msu39k1Rz2zLnqR','20200131184632','53118748',30,489.3,14679,NULL,0),('etSz7KCYJsHb9l4','20200131184632','17795987',30,839.3,25179,NULL,0),('wMmhGkH728xrIbY','20200131184632','25441358',30,839.3,25179,NULL,0),('a6HsvE3ycBgkdfL','20200131184632','66492183',30,839.3,25179,NULL,0),('XRZ0zfk8cc5QJDr','20200131184632','12296577',30,419.3,12579,NULL,0),('idnqNYuENNGtxkO','20200131184632','81685874',30,419.3,12579,NULL,0),('98gY8T5hUWAOmJR','20200131184632','52192942',30,419.3,12579,NULL,0),('Vsv8Y8TJChWJRJt','20200131184632','73316883',30,419.3,12579,NULL,0),('fIqBgoXaNqxN5aW','20200131184632','48349392',30,419.3,12579,NULL,0),('NuE9sd7Dm9l9H3I','20200131184632','35899631',30,419.3,12579,NULL,0),('2YQhZxNwOYdBlYc','20200131184632','64231489',30,419.3,12579,NULL,0),('8kNCj5gf9JekHld','20200131184632','39966796',30,419.3,12579,NULL,0),('Qc2hARVMDjmhkM6','20200131184632','75841999',30,419.3,12579,NULL,0),('DLgPMFKulJ4FuBq','20200131184632','27787243',30,419.3,12579,NULL,0),('MRJFyKWJXas08UM','20200131184632','43823574',30,419.3,12579,NULL,0),('4WYZmmfxWG7PLqR','20200131184632','35387537',30,419.3,12579,NULL,0),('35iC0XMThkvPkti','20200131184632','51814898',30,419.3,12579,NULL,0),('xm6NChyhVLoRa18','20200131184632','24432454',30,419.3,12579,NULL,0),('rt3Qv2o9MLj0aKs','20200131184632','59892815',30,419.3,12579,NULL,0),('9T5KYc96wJkEka','20200131184632','82991274',30,419.3,12579,NULL,0),('BSJ5j6wM50Ye9zo','20200131184632','55426419',30,419.3,12579,NULL,0),('rzmDuwbNvX058Df','20200131184632','72945797',30,419.3,12579,NULL,0),('YFxi0WdvkYv03uz','20200131184632','76394175',30,384.3,11529,NULL,0),('hboKfY87lsUlnRo','20200131184632','26956683',10,384.3,3843,NULL,0),('Ipu7MtUrf4GQ40B','20200131184632','99243581',30,384.3,11529,NULL,0),('sgcXq9vn5XWO90f','20200131184632','11553738',30,384.3,11529,NULL,0),('nILXEYHYGmSM4LN','20200131184632','15444364',30,384.3,11529,NULL,0),('55sXRZTDDQaaeZa','20200131184632','31371779',30,384.3,11529,NULL,0),('ACiLZnkgBrJV5Df','20200131184632','66416536',20,384.3,7686,NULL,0),('G9azWrPsuU9Ktdk','20200131184632','68876788',15,384.3,5764.5,NULL,0),('JdmspCTPjX7tr1w','20200131184632','41767343',15,384.3,5764.5,NULL,0),('M8vnwHVBzETyPER','20200131184632','44556474',30,384.3,11529,NULL,0),('C0ah5TyIOjEow8S','20200131195408','12198562',15,559.3,8389.5,NULL,0),('sBags72tCXcNSQQ','20200131195408','14619457',15,559.3,8389.5,NULL,0),('sJe66RO4u7ZN3no','20200131195408','19351199',15,559.3,8389.5,NULL,0),('XvyrfVBGgsJ6yDu','20200131195408','96482874',101,419.3,42349.3,NULL,0),('FuNvwU8drgb7Mbz','20200131195408','66993765',197,839.3,165342,NULL,0),('U9UugVkLqDYF4y5','20200131195408','14559614',202,419.3,84698.6,NULL,0),('GKahA1pTdMZ1ZyL','20200131195408','86148819',244,384.3,93769.2,NULL,0),('6RpCHNrYi9Pr03J','20200131195408','38998239',98,419.3,41091.4,NULL,0),('cdGzhpRWQ1IBncU','20200131195408','57294745',40,559.3,22372,NULL,0),('sGb1ZHTmiDXNZTf','20200131195408','21853217',195,209.3,40813.5,NULL,0),('IkXDibzljwvwUSc','20200131195408','55778466',155,419.3,64991.5,NULL,0),('y0HUpfMasuoT8UD','20200131195408','22873153',203,839.3,170378,NULL,0),('RU69EvDI8JNhhpZ','20200131195408','32281321',149,384.3,57260.7,NULL,0),('UsfqX4vMhMmMprK','20200131195408','75767819',30,489.3,14679,NULL,0),('mvru1EELGEjXc2R','20200131195408','16174938',232,384.3,89157.6,NULL,0),('1MWpYEzncZpbE7l','20200131195408','55962451',216,384.3,83008.8,NULL,0),('yFHEXUTBAYRYR41','20200131195408','35516844',231,384.3,88773.3,NULL,0),('VfyLrKOXjC0pRKh','20200131195408','53331771',236,384.3,90694.8,NULL,0),('5jQSLQnqCPY4mci','20200131195408','15291467',35,384.3,13450.5,NULL,0),('lIIEMeDt6qbjxSu','20200131195408','29197187',161,419.3,67507.3,NULL,0),('DQ1LTDqZxlFOK3h','20200131195408','76156417',165,384.3,63409.5,NULL,0),('rtXmAVevmr2WEos','20200131195408','42476663',150,419.3,62895,NULL,0),('an5R73vRMFtWxT9','20200131195408','18968634',158,419.3,66249.4,NULL,0),('xJdEW9gamsoH2K4','20200131195408','66953714',165,419.3,69184.5,NULL,0),('dB1U8eVTUcIslmy','20200131195408','45855167',161,419.3,67507.3,NULL,0),('hpRNfcEnLMeIgwj','20200131195408','41211877',99,419.3,41510.7,NULL,0),('OR17nBXyHxZJdFL','20200131195408','59648166',98,419.3,41091.4,NULL,0),('jGp1K2ItJ28SRa7','20200131195408','21826863',100,419.3,41930,NULL,0),('14rWLbKGZkBpqBD','20200131195408','33776845',99,419.3,41510.7,NULL,0),('ElQENRiRtMJadn7','20200131195408','23573952',163,419.3,68345.9,NULL,0),('PugRuy69Z6PFAY4','20200131195408','29684862',171,419.3,71700.3,NULL,0),('MXAlAdEbp2rPegX','20200131195408','43362818',160,419.3,67088,NULL,0),('OJC0RnOl0RqFIxE','20200131195408','71513899',160,419.3,67088,NULL,0),('j7XnddaLwQkmyHv','20200131195408','27711386',175,419.3,73377.5,NULL,0),('BRuIkPUWDEF2H9E','20200131195408','64611434',201,839.3,168699,NULL,0),('8SEgGwsUzYzihIa','20200131195408','85412238',170,559.3,95081,NULL,0),('IVHUeLwxusyD4Mb','20200131195408','76152683',128,944.3,120870,NULL,0),('FnjxsLNCqgsIU3d','20200131195408','19297787',217,489.3,106178,NULL,0),('bnKKDcUj7ASLZXD','20200131195408','42444451',229,489.3,112050,NULL,0),('PfsElZVfUqcwJVz','20200131195408','24475238',227,489.3,111071,NULL,0),('UTsZRIfdqtPZF3p','20200131195408','14487654',166,489.3,81223.8,NULL,0),('nDzGrP45T7dd3CS','20200131195408','14312268',30,489.3,14679,NULL,0),('SaG7ZFLiMvsTnPK','20200131195408','31845465',226,489.3,110582,NULL,0),('lpo00PIIjKH3hy0','20200131195408','56347176',30,489.3,14679,NULL,0),('xocoaxNFCiH6jyb','20200131195408','49649868',30,489.3,14679,NULL,0),('zd1wTipOWC3Aqfs','20200131195408','66616128',30,489.3,14679,NULL,0),('J7XUlIJVC0SWrHP','20200131195408','52235643',41,489.3,20061.3,NULL,0),('Tl72r34xWooomr6','20200131195408','62986341',40,489.3,19572,NULL,0),('OcJpvzt419Vry3I','20200131195408','66741529',40,489.3,19572,NULL,0),('sygG7BFdu2R9CQh','20200131195408','57184611',41,489.3,20061.3,NULL,0),('LmaUpDHBPb11ibg','20200131195408','26227319',40,489.3,19572,NULL,0),('X0YHf9hsvxOK0iN','20200131195408','44488722',42,489.3,20550.6,NULL,0),('DMIDnyTLUzkGkqJ','20200131195408','48286412',187,559.3,104589,NULL,0),('SOugkZNxQfYLVBN','20200131195408','63389971',50,559.3,27965,NULL,0),('N70ZqShfeZwuFDw','20200131195408','16218137',31,559.3,17338.3,NULL,0),('L8MToqN1Sl6zmAs','20200131195408','76447267',189,559.3,105708,NULL,0),('6QMuSIyL2RQqQCN','20200131195408','27343625',193,209.3,40394.9,NULL,0),('6BKrc1LmZEgBF4j','20200131195408','76177618',238,209.3,49813.4,NULL,0),('WoyB5E4tcEz0vf5','20200131195408','51242523',127,174.3,22136.1,NULL,0),('fQvbqeKi44mAXi1','20200131195408','32984827',239,419.3,100213,NULL,0),('F9oQP2yXLGgrq6J','20200131195408','57898978',173,559.3,96758.9,NULL,0),('Vl6ast3YtDHd7z0','20200131195408','34566946',198,209.3,41441.4,NULL,0),('IpKQNiIURIgjYcy','20200131195408','63172769',158,489.3,77309.4,NULL,0),('JGFHxbpALsu3sfJ','20200131195408','61922372',158,489.3,77309.4,NULL,0),('JBIhX2GtANSP7O7','20200131195408','68197248',161,489.3,78777.3,NULL,0),('2J8qWiNtCuqAvQa','20200131195408','27623275',150,489.3,73395,NULL,0),('urXWtjTJFjOt7qN','20200131195408','67699587',12,314.3,3771.6,NULL,0),('cuyMkfXeh78BkaY','20200131195408','76226952',12,314.3,3771.6,NULL,0),('R9mBN9agMFnXbd6','20200131195408','68855634',69,419.3,28931.7,NULL,0),('R4uzrK4BjddncXK','20200131195408','46861688',12,314.3,3771.6,NULL,0),('lctMWZVOoHJWzhB','20200131195408','72933825',172,314.3,54059.6,NULL,0),('TtixZDVZkcD04gz','20200131195408','52812878',185,314.3,58145.5,NULL,0),('65l4pG3Px6ssXoc','20200131195408','31973839',188,314.3,59088.4,NULL,0),('FGbpfg0gIoZaczs','20200131195408','66937463',181,314.3,56888.3,NULL,0),('E39ue9jqCd5c7t9','20200131195408','77288629',120,349.3,41916,NULL,0),('WdkZFUAlo3SyaSc','20200131195408','64357596',125,769.3,96162.5,NULL,0),('kfpmAyLE4OZczfJ','20200131195408','42125549',193,769.3,148475,NULL,0),('8bDltlgT2qJh0oo','20200131195408','35651237',166,769.3,127704,NULL,0),('6bnHhj7cSeitH73','20200131195408','16495644',119,489.3,58226.7,NULL,0),('2BBeIjDECTjkFqh','20200131195408','27138342',231,769.3,177708,NULL,0),('vgnRi5lQKyqSfGk','20200131195408','63319766',55,693,38115,NULL,0),('kvgBQVsPNZIqaRW','20200131195408','47249865',105,489.3,51376.5,NULL,0),('JRLG7z5shFbnXIn','20200131195408','17897237',140,384.3,53802,NULL,0),('n3bfxq93r3eAJpf','20200131195408','59621483',174,419.3,72958.2,NULL,0),('lp8onxwsKtFRqAN','20200131195408','47575149',166,489.3,81223.8,NULL,0),('3P26OdEB4mSQLfA','20200131195408','51943327',367,770,282590,NULL,0),('K8D81HFX7WG8658','20200131195408','12495721',153,454.3,69507.9,NULL,0),('V5D0mIwZiRXiNta','20200131195408','43891483',527,419.3,220971,NULL,0),('Mmx4EHgyQtAix08','20200131195408','75513118',93,419.3,38994.9,NULL,0),('5ForFxxZe8A6W28','20200131195408','47491915',202,209.3,42278.6,NULL,0),('uzpcVnuJMLRE0Yg','20200131195408','76817172',73,419.3,30608.9,NULL,0),('rIw0IGUqJlbY2C2','20200131195408','17189454',237,559.3,132554,NULL,0),('xxrf9SyjbI2xGc6','20200131195408','52328389',134,524.3,70256.2,NULL,0),('q2S0UUiDSuPPkc4','20200131195408','73611942',156,489.3,76330.8,NULL,0),('VysKkSlJVRT8lIl','20200131195408','38766572',162,433.3,70194.6,NULL,0),('sqUcBit0NN71MnN','20200131195408','61458585',110,174.3,19173,NULL,0),('Kf1vkaiCpNmUwoF','20200131195408','43571975',127,174.3,22136.1,NULL,0),('COtVOR9nc8oBrWG','20200131195408','31441889',187,839.3,156949,NULL,0),('JtlEkxbPnknjBv7','20200131195408','98655172',173,454.3,78593.9,NULL,0),('dxSAhnYL0AjtsFd','20200131195408','81468687',417,174.3,72683.1,NULL,0),('oH61LWo1JY7bHWA','20200131195408','47142196',83,419.3,34801.9,NULL,0),('XeNEbxowtFRsajT','20200131195408','19165852',157,699.3,109790,NULL,0),('bRhtTYnTlYurgub','20200131195408','46436494',106,979.3,103806,NULL,0),('rLSE140MuYGI0tH','20200131195408','56527628',150,839.3,125895,NULL,0),('uFq8kkV7af9VRmB','20200131195408','62359995',103,384.3,39582.9,NULL,0),('iRWol6W1Z3Xkn0X','20200131195408','69386854',216,384.3,83008.8,NULL,0),('FUNf4XT1LC4woqZ','20200131195408','12342251',122,944.3,115205,NULL,0),('k7GUirPdggLulwH','20200131195408','44779861',137,944.3,129369,NULL,0),('gFZjKz8xrDllwn5','20200131195408','38462329',157,489.3,76820.1,NULL,0),('ULtll76zRrMEkWe','20200131195408','34166762',99,489.3,48440.7,NULL,0),('s6DEZXn2Fx5tCSx','20200131195408','46568894',103,489.3,50397.9,NULL,0),('36SZMUMVwYx3JZ6','20200131195408','13973482',32,489.3,15657.6,NULL,0),('gHTeVicsJCnPYEQ','20200131195408','25538539',50,693,34650,NULL,0),('1ChSAxHdKcGulCj','20200131195408','65612294',55,693,38115,NULL,0),('Fw8NN8Ykk6FTaAd','20200131195408','58811849',50,693,34650,NULL,0),('BDb9QzcXTnjI7z5','20200131195408','29579574',42,693,29106,NULL,0),('8DHaANXmgfvf5HZ','20200131195408','52353221',42,693,29106,NULL,0),('7B8NYdGfKNxUPvI','20200131195408','11225548',79,665,52535,NULL,0),('R9d7F8mVHI0ZW5G','20200131195408','44975588',60,665,39900,NULL,0),('H1QWg5HrFSID8zr','20200131195408','52729175',80,665,53200,NULL,0),('IvpGepPJgoSM6qj','20200131195408','66714832',70,665,46550,NULL,0),('JqPqouLKLa8proU','20200131195408','35581896',60,573.3,34398,NULL,0),('GT9tAIp1ZIrQYTN','20200131195408','26318697',67,717.5,48072.5,NULL,0),('VCg8fegbPwdQrob','20200131195408','37144252',70,717.5,50225,NULL,0),('rgnoJ71Qd12Po7u','20200131195408','69631976',237,770,182490,NULL,0),('Gvnc9i6uQCA5KLY','20200131195408','56235236',375,770,288750,NULL,0),('TO7nUNHhAkwxEfq','20200131195408','73227748',383,770,294910,NULL,0),('Q8WN6dwsGPnHKeh','20200131195408','32878258',351,770,270270,NULL,0),('tZREPeOxdOxet3B','20200131195408','65346714',371,0,0,NULL,0),('EV68Hkxnd6A51ZL','20200131195408','32958561',159,567,90153,NULL,0),('FT1NJh0AABeEhh','20200131195408','58296557',262,567,148554,NULL,0),('wY3xozg3IWb5ZQ4','20200131195408','66222482',216,525,113400,NULL,0),('yc2Hxqw2cCeEgCA','20200131195408','65873862',170,595,101150,NULL,0),('W3AP7tOU1jvCz9j','20200131195408','23874322',186,595,110670,NULL,0),('dRSTzoDvPnldi7Q','20200131195408','25558411',161,595,95795,NULL,0),('7t4Es9WHVcLQNh0','20200131195408','69472824',204,1015,207060,NULL,0),('uS3hWr7lkzO5Vxy','20200131195408','79774872',228,875,199500,NULL,0),('7FH0FSYWFD6uTtQ','20200131195408','62955453',231,1015,234465,NULL,0),('jlwDzZO34nVei4l','20200131195408','42225661',119,616,73304,NULL,0),('LfDALZRwpioRygZ','20200131195408','63511867',243,616,149688,NULL,0),('bpU2LBxCv1aMDtd','20200131195408','19778245',294,315,92610,NULL,0),('VwcuUXGIfTskAei','20200131195408','75731144',298,315,93870,NULL,0),('S6MmxZolcZb3nfm','20200131195408','92252955',288,315,90720,NULL,0),('HzYkks8q2dCjDCD','20200131195408','51533424',295,315,92925,NULL,0),('lT0z8wOlYqB1joT','20200131195408','62123651',297,315,93555,NULL,0),('NvLlTvUUcLwfuzh','20200131195408','65557469',295,315,92925,NULL,0),('yb4yR1lfc0NezwI','20200131195408','54214699',293,315,92295,NULL,0),('K95WjQH4jkETFzL','20200131195408','74718183',295,315,92925,NULL,0),('nnngc5XDZ1CmV3Z','20200131195408','61659478',51,315,16065,NULL,0),('cDYXZ9nh2MH7jMv','20200131195408','67833341',295,315,92925,NULL,0),('u1ZXW2Ru6rk9V51','20200131195408','14217517',295,315,92925,NULL,0),('tX4IuSGYz8m85LK','20200131195408','63765295',290,315,91350,NULL,0),('q7AGUX9lH4Us2om','20200131195408','46124262',295,315,92925,NULL,0),('rXAlgv4mKtkwqe8','20200131195408','17581624',296,315,93240,NULL,0),('1L52RMOQzWJJFBx','20200131195408','51985917',294,315,92610,NULL,0),('rtLsVrdOUn84ZoY','20200131195408','54491587',294,315,92610,NULL,0),('XfkRW2flHT9ReDU','20200131195408','94241385',291,315,91665,NULL,0),('RvCcdWJ8ldkiXAM','20200131195408','23895138',294,315,92610,NULL,0),('vYsWqFONqThonCs','20200131195408','34564525',294,315,92610,NULL,0),('7Ud9w8T757Zffwf','20200131195408','35873834',288,315,90720,NULL,0),('ti5b5kgIxMeV3IW','20200131195408','26428783',294,315,92610,NULL,0),('kFqEEdBZZ6Wyb3l','20200131195408','72484215',297,315,93555,NULL,0),('7lK2DFbjOOwtjp2','20200131195408','55928712',292,315,91980,NULL,0),('7P8nUBGTbfRxVsq','20200131195408','66796415',294,315,92610,NULL,0),('6pRGgVL95ZMrJwW','20200131195408','84255148',291,315,91665,NULL,0),('Btm9W70dRa9iOUQ','20200131195408','42661445',299,315,94185,NULL,0),('6P6NTchhac8nKGa','20200131195408','77517419',296,315,93240,NULL,0),('gHKJFhFFAWf95uS','20200131195408','28384836',294,315,92610,NULL,0),('LphH33jaZA4WoDd','20200131195408','27694615',297,315,93555,NULL,0),('FkkPtqwyvxKRwFC','20200131195408','71592838',287,315,90405,NULL,0),('ERslYkd9B405Qo8','20200131195408','29311953',296,315,93240,NULL,0),('tthV0QAwFwjPDTT','20200131195408','88326126',300,315,94500,NULL,0),('FamxBBYfAZZ0Vpn','20200131195408','69883829',297,315,93555,NULL,0),('SYXztWGtJLfsbjE','20200131195408','79628194',299,315,94185,NULL,0),('xpeadgb2BcRacmF','20200131195408','65128598',296,315,93240,NULL,0),('G5eduO1180CdseC','20200131195408','59682742',294,315,92610,NULL,0),('xRsRrLyJDzmTQE8','20200131195408','64362819',296,315,93240,NULL,0),('WK32omFfdCffgMA','20200131195408','23483522',296,315,93240,NULL,0),('iBRQU0cHJf7w1N1','20200131195408','66216276',297,315,93555,NULL,0),('yiBqZpSfLvhqRE5','20200131195408','15444644',296,315,93240,NULL,0),('Rm6j4OjTTt2Sfse','20200131195408','56294993',291,315,91665,NULL,0),('WgDXMdDf9ORg6jk','20200131195408','43484992',222,297.5,66045,NULL,0);
/*!40000 ALTER TABLE `product_purchase_details` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary table structure for view `product_report`
--

DROP TABLE IF EXISTS `product_report`;
/*!50001 DROP VIEW IF EXISTS `product_report`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `product_report` (
  `date` tinyint NOT NULL,
  `product_id` tinyint NOT NULL,
  `quantity` tinyint NOT NULL,
  `rate` tinyint NOT NULL,
  `total_amount` tinyint NOT NULL,
  `account` tinyint NOT NULL
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `product_return`
--

DROP TABLE IF EXISTS `product_return`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `product_return` (
  `return_id` varchar(30) CHARACTER SET latin1 NOT NULL,
  `product_id` varchar(20) CHARACTER SET latin1 NOT NULL,
  `invoice_id` varchar(20) CHARACTER SET latin1 NOT NULL,
  `purchase_id` varchar(30) CHARACTER SET latin1 DEFAULT NULL,
  `date_purchase` varchar(20) CHARACTER SET latin1 NOT NULL,
  `date_return` varchar(30) CHARACTER SET latin1 NOT NULL,
  `byy_qty` float NOT NULL,
  `ret_qty` float NOT NULL,
  `customer_id` varchar(20) CHARACTER SET latin1 NOT NULL,
  `supplier_id` varchar(30) CHARACTER SET latin1 NOT NULL,
  `product_rate` float NOT NULL,
  `deduction` float NOT NULL,
  `total_deduct` float NOT NULL,
  `total_tax` float NOT NULL,
  `total_ret_amount` float NOT NULL,
  `net_total_amount` float NOT NULL,
  `reason` text CHARACTER SET latin1 NOT NULL,
  `usablity` int(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_return`
--

LOCK TABLES `product_return` WRITE;
/*!40000 ALTER TABLE `product_return` DISABLE KEYS */;
/*!40000 ALTER TABLE `product_return` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary table structure for view `product_supplier`
--

DROP TABLE IF EXISTS `product_supplier`;
/*!50001 DROP VIEW IF EXISTS `product_supplier`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `product_supplier` (
  `product_id` tinyint NOT NULL,
  `product_name` tinyint NOT NULL,
  `product_model` tinyint NOT NULL,
  `supplier_id` tinyint NOT NULL
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Temporary table structure for view `purchase_report`
--

DROP TABLE IF EXISTS `purchase_report`;
/*!50001 DROP VIEW IF EXISTS `purchase_report`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `purchase_report` (
  `purchase_date` tinyint NOT NULL,
  `chalan_no` tinyint NOT NULL,
  `supplier_id` tinyint NOT NULL,
  `purchase_detail_id` tinyint NOT NULL,
  `purchase_id` tinyint NOT NULL,
  `product_id` tinyint NOT NULL,
  `quantity` tinyint NOT NULL,
  `rate` tinyint NOT NULL,
  `total_amount` tinyint NOT NULL,
  `status` tinyint NOT NULL
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Temporary table structure for view `sales_actual`
--

DROP TABLE IF EXISTS `sales_actual`;
/*!50001 DROP VIEW IF EXISTS `sales_actual`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `sales_actual` (
  `DATE` tinyint NOT NULL,
  `supplier_id` tinyint NOT NULL,
  `sub_total` tinyint NOT NULL,
  `no_transection` tinyint NOT NULL
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Temporary table structure for view `sales_report`
--

DROP TABLE IF EXISTS `sales_report`;
/*!50001 DROP VIEW IF EXISTS `sales_report`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `sales_report` (
  `date` tinyint NOT NULL,
  `invoice_id` tinyint NOT NULL,
  `invoice_details_id` tinyint NOT NULL,
  `customer_id` tinyint NOT NULL,
  `supplier_id` tinyint NOT NULL,
  `product_id` tinyint NOT NULL,
  `product_model` tinyint NOT NULL,
  `product_name` tinyint NOT NULL,
  `quantity` tinyint NOT NULL,
  `sell_rate` tinyint NOT NULL,
  `supplier_rate` tinyint NOT NULL
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Temporary table structure for view `stock_history`
--

DROP TABLE IF EXISTS `stock_history`;
/*!50001 DROP VIEW IF EXISTS `stock_history`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `stock_history` (
  `vdate` tinyint NOT NULL,
  `product_id` tinyint NOT NULL,
  `sell` tinyint NOT NULL,
  `Purchase` tinyint NOT NULL
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `supplier_information`
--

DROP TABLE IF EXISTS `supplier_information`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `supplier_information` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `supplier_id` varchar(100) NOT NULL,
  `supplier_name` varchar(255) NOT NULL,
  `address` varchar(255) NOT NULL,
  `mobile` varchar(100) NOT NULL,
  `details` varchar(255) NOT NULL,
  `status` int(2) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier_information`
--

LOCK TABLES `supplier_information` WRITE;
/*!40000 ALTER TABLE `supplier_information` DISABLE KEYS */;
INSERT INTO `supplier_information` VALUES (2,'P85V2LTWR5XHC4G5ZRFS','GLANZEND UK','188, Kingsbridge drive, Glasgow g44 4jy, ','+441413164490','www.glanzend.co.uk\r\nemail: info@glanzend.co.uk',1);
/*!40000 ALTER TABLE `supplier_information` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier_ledger`
--

DROP TABLE IF EXISTS `supplier_ledger`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `supplier_ledger` (
  `id` int(20) NOT NULL AUTO_INCREMENT,
  `transaction_id` varchar(100) NOT NULL,
  `supplier_id` varchar(100) NOT NULL,
  `chalan_no` varchar(100) DEFAULT NULL,
  `deposit_no` varchar(50) DEFAULT NULL,
  `amount` float NOT NULL,
  `description` varchar(255) NOT NULL,
  `payment_type` varchar(255) NOT NULL,
  `cheque_no` varchar(255) NOT NULL,
  `date` varchar(50) NOT NULL,
  `status` int(2) NOT NULL,
  `d_c` varchar(10) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier_ledger`
--

LOCK TABLES `supplier_ledger` WRITE;
/*!40000 ALTER TABLE `supplier_ledger` DISABLE KEYS */;
INSERT INTO `supplier_ledger` VALUES (5,'20200123135241','P85V2LTWR5XHC4G5ZRFS','GT10001',NULL,413700,'','','','2020-01-23',1,'c'),(4,'20200123135219','P85V2LTWR5XHC4G5ZRFS','GT10001',NULL,413700,'','','','2020-01-23',1,'c'),(3,'1JYG5636BH','P85V2LTWR5XHC4G5ZRFS','Adjustment ',NULL,0,'Previous adjustment with software','NA','NA','2020-01-23',1,'c'),(6,'20200123135306','P85V2LTWR5XHC4G5ZRFS','GT10001',NULL,413700,'','','','2020-01-23',1,'c'),(7,'20200123135452','P85V2LTWR5XHC4G5ZRFS','GT10001',NULL,413700,'','','','2020-01-23',1,'c'),(12,'20200131195408','P85V2LTWR5XHC4G5ZRFS','G10002',NULL,14471600,'','','','2019-10-01',1,'c'),(11,'20200131184632','P85V2LTWR5XHC4G5ZRFS','GT10001',NULL,1654940,'','','','2019-10-01',1,'c');
/*!40000 ALTER TABLE `supplier_ledger` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier_product`
--

DROP TABLE IF EXISTS `supplier_product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `supplier_product` (
  `supplier_pr_id` int(11) NOT NULL AUTO_INCREMENT,
  `product_id` varchar(30) CHARACTER SET latin1 NOT NULL,
  `products_model` varchar(30) CHARACTER SET latin1 DEFAULT NULL,
  `supplier_id` varchar(30) CHARACTER SET latin1 NOT NULL,
  `supplier_price` float NOT NULL,
  PRIMARY KEY (`supplier_pr_id`)
) ENGINE=InnoDB AUTO_INCREMENT=603 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier_product`
--

LOCK TABLES `supplier_product` WRITE;
/*!40000 ALTER TABLE `supplier_product` DISABLE KEYS */;
INSERT INTO `supplier_product` VALUES (82,'17852649','G/T/E/LASHES/A36','P85V2LTWR5XHC4G5ZRFS',314.3),(83,'36497871','G/T/E/LASHES/A36','P85V2LTWR5XHC4G5ZRFS',314.3),(103,'57317359',NULL,'VNF1K363UQXDVU1SMF9E',14500),(272,'29622644','G/T/C/S/TESTER','P85V2LTWR5XHC4G5ZRFS',616),(274,'79774872',NULL,'P85V2LTWR5XHC4G5ZRFS',875),(275,'77517419',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(276,'77288629',NULL,'P85V2LTWR5XHC4G5ZRFS',349.3),(277,'76817172',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(278,'76447267',NULL,'P85V2LTWR5XHC4G5ZRFS',559.3),(279,'76226952',NULL,'P85V2LTWR5XHC4G5ZRFS',314.3),(280,'76177618',NULL,'P85V2LTWR5XHC4G5ZRFS',209.3),(281,'75731144',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(282,'75513118',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(283,'74718183',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(284,'73227748',NULL,'P85V2LTWR5XHC4G5ZRFS',770),(285,'72933825',NULL,'P85V2LTWR5XHC4G5ZRFS',314.3),(286,'72484215',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(287,'71513899',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(288,'69631976',NULL,'P85V2LTWR5XHC4G5ZRFS',770),(289,'69472824',NULL,'P85V2LTWR5XHC4G5ZRFS',1015),(290,'68197248',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(291,'67833341',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(292,'67699587',NULL,'P85V2LTWR5XHC4G5ZRFS',314.3),(293,'66953714',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(294,'66937463',NULL,'P85V2LTWR5XHC4G5ZRFS',314.3),(295,'66796415',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(296,'66741529',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(297,'66714832',NULL,'P85V2LTWR5XHC4G5ZRFS',665),(298,'66616128',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(299,'66222482',NULL,'P85V2LTWR5XHC4G5ZRFS',525),(300,'66216276',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(301,'65873862',NULL,'P85V2LTWR5XHC4G5ZRFS',595),(302,'65612294',NULL,'P85V2LTWR5XHC4G5ZRFS',693),(303,'65557469',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(304,'65346714',NULL,'P85V2LTWR5XHC4G5ZRFS',770),(305,'65128598',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(306,'64611434',NULL,'P85V2LTWR5XHC4G5ZRFS',839.3),(307,'64362819',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(308,'64357596',NULL,'P85V2LTWR5XHC4G5ZRFS',769.3),(309,'63765295',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(310,'63511867',NULL,'P85V2LTWR5XHC4G5ZRFS',616),(311,'63389971',NULL,'P85V2LTWR5XHC4G5ZRFS',559.3),(312,'63172769',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(314,'62955453',NULL,'P85V2LTWR5XHC4G5ZRFS',1015),(315,'62123651',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(316,'61922372',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(317,'61659478',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(318,'61458585',NULL,'P85V2LTWR5XHC4G5ZRFS',174.3),(319,'59682742',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(320,'59648166',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(321,'59621483',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(322,'58811849',NULL,'P85V2LTWR5XHC4G5ZRFS',693),(323,'58296557',NULL,'P85V2LTWR5XHC4G5ZRFS',567),(324,'62986341',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(325,'57184611',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(326,'56527628',NULL,'P85V2LTWR5XHC4G5ZRFS',839.3),(327,'56347176',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(328,'56235236',NULL,'P85V2LTWR5XHC4G5ZRFS',770),(329,'55962451',NULL,'P85V2LTWR5XHC4G5ZRFS',384.3),(330,'55928712',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(331,'54491587',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(332,'54214699',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(333,'53331771',NULL,'P85V2LTWR5XHC4G5ZRFS',384.3),(334,'52812878',NULL,'P85V2LTWR5XHC4G5ZRFS',314.3),(335,'52729175',NULL,'P85V2LTWR5XHC4G5ZRFS',665),(336,'52353221',NULL,'P85V2LTWR5XHC4G5ZRFS',693),(337,'52328389',NULL,'P85V2LTWR5XHC4G5ZRFS',524.3),(338,'52235643',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(339,'51985917',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(340,'51533424',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(341,'49649868',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(342,'48286412',NULL,'P85V2LTWR5XHC4G5ZRFS',559.3),(343,'47575149',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(345,'46861688',NULL,'P85V2LTWR5XHC4G5ZRFS',314.3),(346,'46568894',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(347,'46436494',NULL,'P85V2LTWR5XHC4G5ZRFS',979.3),(348,'46124262',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(349,'45855167',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(350,'44975588',NULL,'P85V2LTWR5XHC4G5ZRFS',665),(351,'44779861',NULL,'P85V2LTWR5XHC4G5ZRFS',944.3),(352,'44488722',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(353,'43891483',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(354,'43571975',NULL,'P85V2LTWR5XHC4G5ZRFS',174.3),(355,'43362818',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(356,'42661445',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(357,'42476663',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(358,'42444451',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(359,'42225661',NULL,'P85V2LTWR5XHC4G5ZRFS',616),(360,'42125549',NULL,'P85V2LTWR5XHC4G5ZRFS',769.3),(361,'41211877',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(362,'37144252',NULL,'P85V2LTWR5XHC4G5ZRFS',717.5),(363,'35873834',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(364,'47142196',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(365,'35651237',NULL,'P85V2LTWR5XHC4G5ZRFS',769.3),(366,'35581896',NULL,'P85V2LTWR5XHC4G5ZRFS',573.3),(367,'35516844',NULL,'P85V2LTWR5XHC4G5ZRFS',384.3),(368,'34566946',NULL,'P85V2LTWR5XHC4G5ZRFS',209.3),(369,'34564525',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(370,'34166762',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(371,'33776845',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(372,'32958561',NULL,'P85V2LTWR5XHC4G5ZRFS',567),(373,'32878258',NULL,'P85V2LTWR5XHC4G5ZRFS',770),(374,'32281321',NULL,'P85V2LTWR5XHC4G5ZRFS',384.3),(375,'31973839',NULL,'P85V2LTWR5XHC4G5ZRFS',314.3),(376,'31441889',NULL,'P85V2LTWR5XHC4G5ZRFS',839.3),(377,'29684862',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(378,'29579574',NULL,'P85V2LTWR5XHC4G5ZRFS',693),(379,'29311953',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(380,'29197187',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(381,'28384836',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(382,'27711386',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(383,'27694615',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(384,'27623275',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(385,'27343625',NULL,'P85V2LTWR5XHC4G5ZRFS',209.3),(386,'27138342',NULL,'P85V2LTWR5XHC4G5ZRFS',769.3),(387,'26428783',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(388,'26318697',NULL,'P85V2LTWR5XHC4G5ZRFS',717.5),(389,'26227319',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(390,'25558411',NULL,'P85V2LTWR5XHC4G5ZRFS',595),(391,'25538539',NULL,'P85V2LTWR5XHC4G5ZRFS',693),(392,'24475238',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(393,'23874322',NULL,'P85V2LTWR5XHC4G5ZRFS',595),(394,'23573952',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(395,'23483522',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(396,'21826863',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(397,'19778245',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(398,'19297787',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(399,'19165852',NULL,'P85V2LTWR5XHC4G5ZRFS',699.3),(400,'18968634',NULL,'P85V2LTWR5XHC4G5ZRFS',419.3),(401,'17897237',NULL,'P85V2LTWR5XHC4G5ZRFS',384.3),(402,'17189454',NULL,'P85V2LTWR5XHC4G5ZRFS',559.3),(403,'11225548',NULL,'P85V2LTWR5XHC4G5ZRFS',665),(404,'16495644',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(405,'16218137',NULL,'P85V2LTWR5XHC4G5ZRFS',559.3),(406,'16174938',NULL,'P85V2LTWR5XHC4G5ZRFS',384.3),(407,'15444644',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(408,'14487654',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(409,'14312268',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(410,'14217517',NULL,'P85V2LTWR5XHC4G5ZRFS',315),(411,'12495721',NULL,'P85V2LTWR5XHC4G5ZRFS',454.3),(412,'12342251',NULL,'P85V2LTWR5XHC4G5ZRFS',944.3),(413,'51943327','5021769215479','P85V2LTWR5XHC4G5ZRFS',770),(414,'63319766','5021752003694','P85V2LTWR5XHC4G5ZRFS',693),(415,'47249865','5021769267065','P85V2LTWR5XHC4G5ZRFS',489.3),(416,'76152683','5021769267362','P85V2LTWR5XHC4G5ZRFS',944.3),(417,'85412238','5021769267157','P85V2LTWR5XHC4G5ZRFS',559.3),(418,'57898978','5021769267164','P85V2LTWR5XHC4G5ZRFS',559.3),(419,'32984827','5021769277101','P85V2LTWR5XHC4G5ZRFS',419.3),(421,'81468687','5021769255017','P85V2LTWR5XHC4G5ZRFS',174.3),(422,'51242523',NULL,'P85V2LTWR5XHC4G5ZRFS',174.3),(423,'98655172','5021769215141','P85V2LTWR5XHC4G5ZRFS',454.3),(424,'68855634','5021752135647','P85V2LTWR5XHC4G5ZRFS',419.3),(425,'38766572','5021769265122','P85V2LTWR5XHC4G5ZRFS',433.3),(426,'73611942','5021769285151','P85V2LTWR5XHC4G5ZRFS',489.3),(427,'47491915','5021785422257','P85V2LTWR5XHC4G5ZRFS',209.3),(428,'21853217','5021788897458','P85V2LTWR5XHC4G5ZRFS',209.3),(429,'57294745','5021756952110','P85V2LTWR5XHC4G5ZRFS',559.3),(430,'75767819','5021778444136','P85V2LTWR5XHC4G5ZRFS',489.3),(431,'13973482','5021775489758','P85V2LTWR5XHC4G5ZRFS',489.3),(432,'38462329','5021769276210','P85V2LTWR5XHC4G5ZRFS',489.3),(434,'31845465',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(435,'66993765','5021769276142','P85V2LTWR5XHC4G5ZRFS',839.3),(436,'22873153','5021769276159','P85V2LTWR5XHC4G5ZRFS',839.3),(437,'55778466','5021755487965','P85V2LTWR5XHC4G5ZRFS',419.3),(438,'96482874','5021721321323','P85V2LTWR5XHC4G5ZRFS',419.3),(439,'38998239','5021769878988','P85V2LTWR5XHC4G5ZRFS',419.3),(440,'14559614','5021769266143','P85V2LTWR5XHC4G5ZRFS',419.3),(441,'76156417','5021769296041','P85V2LTWR5XHC4G5ZRFS',384.3),(442,'15291467','5021769256144','P85V2LTWR5XHC4G5ZRFS',384.3),(443,'69386854','5021769246091','P85V2LTWR5XHC4G5ZRFS',384.3),(444,'62359995','5021769256139','P85V2LTWR5XHC4G5ZRFS',384.3),(445,'86148819','5021769206019','P85V2LTWR5XHC4G5ZRFS',384.3),(446,'92252955','5021769271161','P85V2LTWR5XHC4G5ZRFS',315),(447,'17581624','5021769261049','P85V2LTWR5XHC4G5ZRFS',315),(448,'94241385','5021769271291','P85V2LTWR5XHC4G5ZRFS',315),(449,'23895138','5021769261087','P85V2LTWR5XHC4G5ZRFS',315),(450,'84255148','5021769241126','P85V2LTWR5XHC4G5ZRFS',315),(451,'71592838','5021769231561','P85V2LTWR5XHC4G5ZRFS',315),(452,'88326126','5021769241089','P85V2LTWR5XHC4G5ZRFS',315),(453,'69883829','5021769241027','P85V2LTWR5XHC4G5ZRFS',315),(454,'79628194','5021769241195','P85V2LTWR5XHC4G5ZRFS',315),(455,'56294993','5021769271253','P85V2LTWR5XHC4G5ZRFS',315),(456,'43484992','5021769253013','P85V2LTWR5XHC4G5ZRFS',297.5),(457,'62166484','8662200111119 - T','P85V2LTWR5XHC4G5ZRFS',616),(458,'67365729','8662200222228 - T','P85V2LTWR5XHC4G5ZRFS',616),(461,'96724851','8662200222259 - T','P85V2LTWR5XHC4G5ZRFS',1015),(462,'96118932',NULL,'P85V2LTWR5XHC4G5ZRFS',1015),(463,'19722853',NULL,'P85V2LTWR5XHC4G5ZRFS',875),(464,'16886114','8662200222266 - T','P85V2LTWR5XHC4G5ZRFS',595),(465,'86894214','8662200222853 - T','P85V2LTWR5XHC4G5ZRFS',595),(466,'17979528','8662200222860 - T','P85V2LTWR5XHC4G5ZRFS',595),(467,'54385866','8662200222273 - T','P85V2LTWR5XHC4G5ZRFS',525),(468,'37897494','8662200222280 - T','P85V2LTWR5XHC4G5ZRFS',567),(469,'39817635','8662200222297 - T','P85V2LTWR5XHC4G5ZRFS',567),(470,'31152412','8662200221252 - T','P85V2LTWR5XHC4G5ZRFS',0),(471,'73663383','8662200221269 - T','P85V2LTWR5XHC4G5ZRFS',0),(472,'89476856','8662200221276 - T','P85V2LTWR5XHC4G5ZRFS',0),(473,'29861256','8662200221283 - T','P85V2LTWR5XHC4G5ZRFS',0),(474,'85357756','8662200221290 - T','P85V2LTWR5XHC4G5ZRFS',0),(475,'89217173','8662200221306 - T','P85V2LTWR5XHC4G5ZRFS',0),(476,'26593387','8662200222105 - T','P85V2LTWR5XHC4G5ZRFS',717.5),(477,'14357447','8662200222877 - T','P85V2LTWR5XHC4G5ZRFS',717.5),(478,'38242799','8662200502177 - T','P85V2LTWR5XHC4G5ZRFS',573.3),(479,'91716273','8662200222204 - T','P85V2LTWR5XHC4G5ZRFS',665),(480,'92163556','8662200222884 - T','P85V2LTWR5XHC4G5ZRFS',665),(481,'16697186','8662200222891 - T','P85V2LTWR5XHC4G5ZRFS',665),(482,'47944384','8662200222907 - T','P85V2LTWR5XHC4G5ZRFS',665),(484,'54488297','8662200222914 - T','P85V2LTWR5XHC4G5ZRFS',693),(485,'95869884',NULL,'P85V2LTWR5XHC4G5ZRFS',693),(486,'77757352','8662200222921 - T','P85V2LTWR5XHC4G5ZRFS',693),(487,'33237645','8662200222938 - T','P85V2LTWR5XHC4G5ZRFS',693),(488,'51828494','8662200222945 - T','P85V2LTWR5XHC4G5ZRFS',693),(489,'71391422','8662200222952 - T','P85V2LTWR5XHC4G5ZRFS',693),(490,'81867689','8662200222129 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(491,'87552587','8662200222969 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(492,'78122882','8662200222976 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(493,'55182652','8662200222136 - T','P85V2LTWR5XHC4G5ZRFS',944.3),(494,'72738337','8662200222143','P85V2LTWR5XHC4G5ZRFS',944.3),(495,'91176321','8662200222150','P85V2LTWR5XHC4G5ZRFS',944.3),(496,'95116862','8662200222167 - T','P85V2LTWR5XHC4G5ZRFS',559.3),(497,'45635275','8662200222174 - T','P85V2LTWR5XHC4G5ZRFS',559.3),(498,'46454148','8662200222181 - T','P85V2LTWR5XHC4G5ZRFS',839.3),(499,'45772378','8662200222198 - T','P85V2LTWR5XHC4G5ZRFS',699.3),(500,'78145559','8662200222235 - T','P85V2LTWR5XHC4G5ZRFS',979.3),(501,'98228499','8662200222242 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(502,'32356294','8662200221313 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(503,'54795555','8662200222303 - T','P85V2LTWR5XHC4G5ZRFS',839.3),(505,'13411811','8662200222983 - T','P85V2LTWR5XHC4G5ZRFS',174.3),(506,'82128627',NULL,'P85V2LTWR5XHC4G5ZRFS',174.3),(507,'49614498','8662200222990 - T','P85V2LTWR5XHC4G5ZRFS',174.3),(508,'12826472','8662200222327 - T','P85V2LTWR5XHC4G5ZRFS',174.3),(509,'93729414','8662200222334 - T','P85V2LTWR5XHC4G5ZRFS',454.3),(510,'58885451','8662200222341 - T ','P85V2LTWR5XHC4G5ZRFS',524.3),(511,'87572955','8662200222358 - T','P85V2LTWR5XHC4G5ZRFS',559.3),(512,'92888129','8662200222365 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(513,'36518381','8662200221009 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(514,'63657454','8662200221016 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(515,'32559217','8662200222372 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(516,'83847398','8662200222389 - T','P85V2LTWR5XHC4G5ZRFS',454.3),(517,'32856757','8662200222396 - T','P85V2LTWR5XHC4G5ZRFS',433.3),(519,'36826337',NULL,'P85V2LTWR5XHC4G5ZRFS',489.3),(520,'21563758','8662200222419 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(521,'83635849','8662200222426 - T','P85V2LTWR5XHC4G5ZRFS',384.3),(522,'37194585','8662200222433 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(523,'15649581','8662200222440 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(524,'54854648','8662200222457 - T','P85V2LTWR5XHC4G5ZRFS',769.3),(525,'26247277','8662200222464 - T','P85V2LTWR5XHC4G5ZRFS',769.3),(526,'85299512','8662200222471 - T','P85V2LTWR5XHC4G5ZRFS',769.3),(527,'58245217','8662200222488 - T','P85V2LTWR5XHC4G5ZRFS',769.3),(528,'73881177','8662200502115 - T','P85V2LTWR5XHC4G5ZRFS',559.3),(529,'28544221','8662200502191 - T','P85V2LTWR5XHC4G5ZRFS',559.3),(530,'99785782','8662200502108 - T','P85V2LTWR5XHC4G5ZRFS',559.3),(531,'21315922','8662200222495 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(532,'39513595','8662200221023 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(533,'88636935','8662200221030 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(534,'95944555','8662200221047 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(535,'78184648','8662200222501 - T','P85V2LTWR5XHC4G5ZRFS',209.3),(536,'75642535','8662200221054 - T','P85V2LTWR5XHC4G5ZRFS',209.3),(537,'86315763','8662200221061 - T','P85V2LTWR5XHC4G5ZRFS',209.3),(538,'46821498','8662200221078 - T','P85V2LTWR5XHC4G5ZRFS',209.3),(539,'59333774','8662200221085 - T','P85V2LTWR5XHC4G5ZRFS',209.3),(540,'75169723','8662200222518 - T','P85V2LTWR5XHC4G5ZRFS',559.3),(541,'41551268','8662200221092 - T','P85V2LTWR5XHC4G5ZRFS',559.3),(542,'36365224','8662200221108 - T','P85V2LTWR5XHC4G5ZRFS',559.3),(543,'39942618','8662200221115 - T','P85V2LTWR5XHC4G5ZRFS',559.3),(544,'35978473','8662200221122 - T','P85V2LTWR5XHC4G5ZRFS',559.3),(545,'19554935','8662200222525 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(546,'23461782','8662200221153 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(547,'34433392','8662200221177 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(548,'78283483','8662200221139 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(549,'53668145','8662200221160 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(550,'53823878','8662200221146 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(551,'29583586','8662200222525 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(552,'29827385','8662200221153 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(553,'24357823','8662200221177 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(554,'59938141','8662200221139 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(555,'73746255','8662200221160 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(556,'33798979','8662200221146 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(557,'38743941','8662200222532 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(558,'47981972','8662200222549 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(559,'93232524','8662200221184 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(560,'62789466','8662200221191 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(561,'19338288','8662200221214 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(562,'53118748','8662200221221 - T','P85V2LTWR5XHC4G5ZRFS',489.3),(563,'17795987','8662200222563 - T','P85V2LTWR5XHC4G5ZRFS',839.3),(564,'25441358','8662200221238 - T','P85V2LTWR5XHC4G5ZRFS',839.3),(565,'66492183','8662200221245 - T','P85V2LTWR5XHC4G5ZRFS',839.3),(566,'12296577','8662200222693 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(567,'81685874','8662200222709 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(568,'52192942','8662200222716 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(569,'73316883','8662200222723 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(570,'48349392','8662200222730 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(571,'35899631','8662200222747 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(572,'64231489','8662200222754 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(573,'39966796','8662200222761 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(574,'75841999','8662200222778 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(575,'27787243','8662200222785 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(576,'43823574','8662200222792 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(577,'35387537','8662200222808 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(578,'51814898','8662200222815 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(579,'24432454','8662200222822 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(580,'59892815','8662200222839 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(581,'82991274','8662200222846 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(582,'55426419','8662200222570 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(583,'72945797','8662200222587 - T','P85V2LTWR5XHC4G5ZRFS',419.3),(584,'76394175','8662200222594 - T','P85V2LTWR5XHC4G5ZRFS',384.3),(585,'26956683','8662200222600 - T','P85V2LTWR5XHC4G5ZRFS',384.3),(586,'99243581','8662200222617 - T','P85V2LTWR5XHC4G5ZRFS',384.3),(587,'11553738','8662200222624 - T','P85V2LTWR5XHC4G5ZRFS',384.3),(588,'15444364','8662200222631 - T','P85V2LTWR5XHC4G5ZRFS',384.3),(589,'31371779','8662200222648 - T','P85V2LTWR5XHC4G5ZRFS',384.3),(590,'66416536','8662200222655 - T','P85V2LTWR5XHC4G5ZRFS',384.3),(591,'68876788','8662200222662 - T','P85V2LTWR5XHC4G5ZRFS',384.3),(592,'41767343','8662200222679 - T','P85V2LTWR5XHC4G5ZRFS',384.3),(593,'44556474','8662200222686 - T','P85V2LTWR5XHC4G5ZRFS',384.3),(594,'19351199','5021769257066','P85V2LTWR5XHC4G5ZRFS',559.3),(595,'14619457','5021769277033','P85V2LTWR5XHC4G5ZRFS',559.3),(596,'12198562','5021769277026','P85V2LTWR5XHC4G5ZRFS',559.3),(597,'87474976','8662200222525','P85V2LTWR5XHC4G5ZRFS',489.3),(598,'36439921','8662200221153','P85V2LTWR5XHC4G5ZRFS',489.3),(599,'89424962','8662200221177','P85V2LTWR5XHC4G5ZRFS',489.3),(600,'79488192','8662200221139','P85V2LTWR5XHC4G5ZRFS',489.3),(601,'13866375','8662200221160','P85V2LTWR5XHC4G5ZRFS',489.3),(602,'58648897','8662200221146','P85V2LTWR5XHC4G5ZRFS',489.3);
/*!40000 ALTER TABLE `supplier_product` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `synchronizer_setting`
--

DROP TABLE IF EXISTS `synchronizer_setting`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `synchronizer_setting` (
  `id` int(11) NOT NULL,
  `hostname` varchar(100) NOT NULL,
  `username` varchar(100) NOT NULL,
  `password` varchar(100) NOT NULL,
  `port` varchar(10) NOT NULL,
  `debug` varchar(10) NOT NULL,
  `project_root` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `synchronizer_setting`
--

LOCK TABLES `synchronizer_setting` WRITE;
/*!40000 ALTER TABLE `synchronizer_setting` DISABLE KEYS */;
/*!40000 ALTER TABLE `synchronizer_setting` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tax_information`
--

DROP TABLE IF EXISTS `tax_information`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tax_information` (
  `tax_id` varchar(15) NOT NULL,
  `tax` float DEFAULT NULL,
  `status` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tax_information`
--

LOCK TABLES `tax_information` WRITE;
/*!40000 ALTER TABLE `tax_information` DISABLE KEYS */;
/*!40000 ALTER TABLE `tax_information` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `transection`
--

DROP TABLE IF EXISTS `transection`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `transection` (
  `id` int(20) NOT NULL AUTO_INCREMENT,
  `transaction_id` varchar(30) NOT NULL,
  `date_of_transection` varchar(30) NOT NULL,
  `transection_type` varchar(30) NOT NULL,
  `transection_category` varchar(30) NOT NULL,
  `transection_mood` varchar(25) NOT NULL,
  `amount` varchar(20) NOT NULL,
  `pay_amount` int(30) DEFAULT NULL,
  `description` varchar(255) NOT NULL,
  `relation_id` varchar(30) NOT NULL,
  `is_transaction` int(2) NOT NULL COMMENT '0 = invoice and 1 = transaction',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `transection`
--

LOCK TABLES `transection` WRITE;
/*!40000 ALTER TABLE `transection` DISABLE KEYS */;
/*!40000 ALTER TABLE `transection` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `units`
--

DROP TABLE IF EXISTS `units`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `units` (
  `unit_id` varchar(255) CHARACTER SET latin1 NOT NULL,
  `unit_name` varchar(255) CHARACTER SET latin1 NOT NULL,
  `status` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `units`
--

LOCK TABLES `units` WRITE;
/*!40000 ALTER TABLE `units` DISABLE KEYS */;
INSERT INTO `units` VALUES ('I3QT1TR3VJIYAZX','Piece',1),('W9YUY1HJQM9IEFT','KG',1),('XM4H48345SO6M94','test',1);
/*!40000 ALTER TABLE `units` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_login`
--

DROP TABLE IF EXISTS `user_login`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `user_login` (
  `user_id` varchar(15) NOT NULL,
  `username` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `user_type` int(2) NOT NULL,
  `security_code` varchar(255) NOT NULL,
  `status` int(2) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_login`
--

LOCK TABLES `user_login` WRITE;
/*!40000 ALTER TABLE `user_login` DISABLE KEYS */;
INSERT INTO `user_login` VALUES ('1','test@test.com','41d99b369894eb1ec3f461135132d8bb',1,'',1),('oZTpXAmq4itvJmY','s@demo.com','41d99b369894eb1ec3f461135132d8bb',2,'',1),('S0U09GyRscAMSOS','admin@obs.com','00b8fead375ca5c76d14ecf52f4c4002',1,'',1),('oGCimGc2P3sU6NK','hamza@glanzend.com','cb196b7b90a400a82297f0951eebc949',1,'',1),('o8YXJMZeZd5UfTi','khadija@glanzend.com','e3c23a8daf39c08c129f8a50180946ef',1,'',1);
/*!40000 ALTER TABLE `user_login` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `user_id` varchar(15) NOT NULL,
  `last_name` varchar(255) NOT NULL,
  `first_name` varchar(255) NOT NULL,
  `gender` int(2) NOT NULL,
  `date_of_birth` varchar(255) NOT NULL,
  `logo` varchar(250) DEFAULT NULL,
  `status` int(2) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES ('1','Doye','John',1,'','http://softest3.bdtask.com/wholesale-v3/assets/dist/img/profile_picture/20d549f44403f065ff12b35a1f09817f.jpg',1),('oZTpXAmq4itvJmY','khan','Sourav',0,'',NULL,1),('S0U09GyRscAMSOS','admin','admin',0,'',NULL,1),('oGCimGc2P3sU6NK','butt','hamza',0,'',NULL,1),('o8YXJMZeZd5UfTi','khadija','khadija',0,'',NULL,1);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary table structure for view `view_customer_transection`
--

DROP TABLE IF EXISTS `view_customer_transection`;
/*!50001 DROP VIEW IF EXISTS `view_customer_transection`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `view_customer_transection` (
  `transaction_id` tinyint NOT NULL,
  `customer_name` tinyint NOT NULL,
  `invoice_no` tinyint NOT NULL
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Temporary table structure for view `view_person_transection`
--

DROP TABLE IF EXISTS `view_person_transection`;
/*!50001 DROP VIEW IF EXISTS `view_person_transection`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `view_person_transection` (
  `transaction_id` tinyint NOT NULL,
  `person_name` tinyint NOT NULL
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Temporary table structure for view `view_supplier_transection`
--

DROP TABLE IF EXISTS `view_supplier_transection`;
/*!50001 DROP VIEW IF EXISTS `view_supplier_transection`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `view_supplier_transection` (
  `transaction_id` tinyint NOT NULL,
  `supplier_name` tinyint NOT NULL
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Temporary table structure for view `view_transection`
--

DROP TABLE IF EXISTS `view_transection`;
/*!50001 DROP VIEW IF EXISTS `view_transection`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `view_transection` (
  `transaction_id` tinyint NOT NULL,
  `date_of_transection` tinyint NOT NULL,
  `amount` tinyint NOT NULL,
  `pay_amount` tinyint NOT NULL,
  `invoice_no` tinyint NOT NULL,
  `customer_name` tinyint NOT NULL,
  `supplier_name` tinyint NOT NULL,
  `person_name` tinyint NOT NULL,
  `transection_category` tinyint NOT NULL,
  `transection_type` tinyint NOT NULL,
  `transection_mood` tinyint NOT NULL,
  `description` tinyint NOT NULL,
  `relation_id` tinyint NOT NULL
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `web_setting`
--

DROP TABLE IF EXISTS `web_setting`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `web_setting` (
  `setting_id` int(11) NOT NULL,
  `logo` varchar(255) DEFAULT NULL,
  `invoice_logo` varchar(255) DEFAULT NULL,
  `favicon` varchar(255) DEFAULT NULL,
  `currency` varchar(10) DEFAULT NULL,
  `currency_position` varchar(10) DEFAULT NULL,
  `footer_text` varchar(255) DEFAULT NULL,
  `language` varchar(255) DEFAULT NULL,
  `rtr` varchar(255) DEFAULT NULL,
  `captcha` int(11) DEFAULT '1' COMMENT '0=active,1=inactive',
  `site_key` varchar(250) DEFAULT NULL,
  `secret_key` varchar(250) DEFAULT NULL,
  `discount_type` int(11) DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `web_setting`
--

LOCK TABLES `web_setting` WRITE;
/*!40000 ALTER TABLE `web_setting` DISABLE KEYS */;
INSERT INTO `web_setting` VALUES (1,'http://202.166.166.100/oerp/my-assets/image/logo/bd34f76a722e0ca487a735ed2c025eca.png','http://202.166.166.100/oerp/my-assets/image/logo/21418adc7f6d2cf334f55557317e7c23.png','http://202.166.166.100/oerp/my-assets/image/logo/40ebf86b992849a3898c65acfa164f0b.jpeg','₨','0','Copyright Glanzend Pvt. Ltd. All rights reserved.','english','0',1,'','',1);
/*!40000 ALTER TABLE `web_setting` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Final view structure for view `customer_transection_summary`
--

/*!50001 DROP TABLE IF EXISTS `customer_transection_summary`*/;
/*!50001 DROP VIEW IF EXISTS `customer_transection_summary`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = latin1 */;
/*!50001 SET character_set_results     = latin1 */;
/*!50001 SET collation_connection      = latin1_swedish_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `customer_transection_summary` AS select `customer_information`.`customer_name` AS `customer_name`,`customer_ledger`.`customer_id` AS `customer_id`,'credit' AS `type`,sum(-(`customer_ledger`.`amount`)) AS `amount` from (`customer_ledger` join `customer_information` on((`customer_information`.`customer_id` = `customer_ledger`.`customer_id`))) where (isnull(`customer_ledger`.`receipt_no`) and (`customer_ledger`.`status` = 1)) group by `customer_ledger`.`customer_id` union all select `customer_information`.`customer_name` AS `customer_name`,`customer_ledger`.`customer_id` AS `customer_id`,'debit' AS `type`,sum(`customer_ledger`.`amount`) AS `amount` from (`customer_ledger` join `customer_information` on((`customer_information`.`customer_id` = `customer_ledger`.`customer_id`))) where (isnull(`customer_ledger`.`invoice_no`) and (`customer_ledger`.`status` = 1)) group by `customer_ledger`.`customer_id` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `product_report`
--

/*!50001 DROP TABLE IF EXISTS `product_report`*/;
/*!50001 DROP VIEW IF EXISTS `product_report`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = latin1 */;
/*!50001 SET character_set_results     = latin1 */;
/*!50001 SET collation_connection      = latin1_swedish_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `product_report` AS select `purchase_report`.`purchase_date` AS `date`,`purchase_report`.`product_id` AS `product_id`,`purchase_report`.`quantity` AS `quantity`,`purchase_report`.`rate` AS `rate`,`purchase_report`.`total_amount` AS `total_amount`,'a' AS `account` from `purchase_report` union all select `sales_report`.`date` AS `date`,`sales_report`.`product_id` AS `product_id`,-(`sales_report`.`quantity`) AS `-quantity`,`sales_report`.`supplier_rate` AS `rate`,(`sales_report`.`quantity` * `sales_report`.`supplier_rate`) AS `total_amount`,'b' AS `account` from `sales_report` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `product_supplier`
--

/*!50001 DROP TABLE IF EXISTS `product_supplier`*/;
/*!50001 DROP VIEW IF EXISTS `product_supplier`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = latin1 */;
/*!50001 SET character_set_results     = latin1 */;
/*!50001 SET collation_connection      = latin1_swedish_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `product_supplier` AS select `b`.`product_id` AS `product_id`,`c`.`product_name` AS `product_name`,`c`.`product_model` AS `product_model`,`a`.`supplier_id` AS `supplier_id` from ((`product_purchase` `a` join `product_purchase_details` `b`) join `product_information` `c`) where ((`a`.`purchase_id` = `b`.`purchase_id`) and (`c`.`product_id` = `b`.`product_id`)) group by `b`.`product_id` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `purchase_report`
--

/*!50001 DROP TABLE IF EXISTS `purchase_report`*/;
/*!50001 DROP VIEW IF EXISTS `purchase_report`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = latin1 */;
/*!50001 SET character_set_results     = latin1 */;
/*!50001 SET collation_connection      = latin1_swedish_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `purchase_report` AS select `product_purchase`.`purchase_date` AS `purchase_date`,`product_purchase`.`chalan_no` AS `chalan_no`,`product_purchase`.`supplier_id` AS `supplier_id`,`product_purchase_details`.`purchase_detail_id` AS `purchase_detail_id`,`product_purchase_details`.`purchase_id` AS `purchase_id`,`product_purchase_details`.`product_id` AS `product_id`,`product_purchase_details`.`quantity` AS `quantity`,`product_purchase_details`.`rate` AS `rate`,`product_purchase_details`.`total_amount` AS `total_amount`,`product_purchase_details`.`status` AS `status` from (`product_purchase_details` left join `product_purchase` on((`product_purchase_details`.`purchase_id` = `product_purchase`.`purchase_id`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `sales_actual`
--

/*!50001 DROP TABLE IF EXISTS `sales_actual`*/;
/*!50001 DROP VIEW IF EXISTS `sales_actual`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = latin1 */;
/*!50001 SET character_set_results     = latin1 */;
/*!50001 SET collation_connection      = latin1_swedish_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `sales_actual` AS select `sales_report`.`date` AS `DATE`,`sales_report`.`supplier_id` AS `supplier_id`,sum((`sales_report`.`quantity` * -(`sales_report`.`supplier_rate`))) AS `sub_total`,sum(`sales_report`.`quantity`) AS `no_transection` from `sales_report` group by `sales_report`.`date`,`sales_report`.`supplier_id` union all select `supplier_ledger`.`date` AS `date`,`supplier_ledger`.`supplier_id` AS `supplier_id`,`supplier_ledger`.`amount` AS `sub_total`,`supplier_ledger`.`description` AS `no_transection` from `supplier_ledger` where isnull(`supplier_ledger`.`chalan_no`) group by `supplier_ledger`.`date`,`supplier_ledger`.`description`,`supplier_ledger`.`supplier_id` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `sales_report`
--

/*!50001 DROP TABLE IF EXISTS `sales_report`*/;
/*!50001 DROP VIEW IF EXISTS `sales_report`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = latin1 */;
/*!50001 SET character_set_results     = latin1 */;
/*!50001 SET collation_connection      = latin1_swedish_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `sales_report` AS select `b`.`date` AS `date`,`b`.`invoice_id` AS `invoice_id`,`a`.`invoice_details_id` AS `invoice_details_id`,`b`.`customer_id` AS `customer_id`,`c`.`supplier_id` AS `supplier_id`,`a`.`product_id` AS `product_id`,`c`.`product_model` AS `product_model`,`c`.`product_name` AS `product_name`,`a`.`quantity` AS `quantity`,`a`.`rate` AS `sell_rate`,`a`.`supplier_rate` AS `supplier_rate` from ((`invoice_details` `a` join `invoice` `b`) join `product_supplier` `c`) where ((`a`.`invoice_id` = `b`.`invoice_id`) and (`a`.`product_id` = `c`.`product_id`)) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `stock_history`
--

/*!50001 DROP TABLE IF EXISTS `stock_history`*/;
/*!50001 DROP VIEW IF EXISTS `stock_history`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = latin1 */;
/*!50001 SET character_set_results     = latin1 */;
/*!50001 SET collation_connection      = latin1_swedish_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `stock_history` AS select `invoice`.`date` AS `vdate`,`invoice_details`.`product_id` AS `product_id`,sum(`invoice_details`.`quantity`) AS `sell`,0 AS `Purchase` from (`invoice_details` join `invoice` on((`invoice_details`.`invoice_id` = `invoice`.`invoice_id`))) group by `invoice_details`.`product_id`,`invoice`.`date` union select `product_purchase`.`purchase_date` AS `purchase_date`,`product_purchase_details`.`product_id` AS `product_id`,0 AS `0`,sum(`product_purchase_details`.`quantity`) AS `purchase` from (`product_purchase_details` join `product_purchase` on((`product_purchase_details`.`purchase_id` = `product_purchase`.`purchase_id`))) group by `product_purchase_details`.`product_id`,`product_purchase`.`purchase_date` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `view_customer_transection`
--

/*!50001 DROP TABLE IF EXISTS `view_customer_transection`*/;
/*!50001 DROP VIEW IF EXISTS `view_customer_transection`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = latin1 */;
/*!50001 SET character_set_results     = latin1 */;
/*!50001 SET collation_connection      = latin1_swedish_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `view_customer_transection` AS (select `i`.`transaction_id` AS `transaction_id`,`j`.`customer_name` AS `customer_name`,`q`.`invoice_no` AS `invoice_no` from ((`transection` `i` left join `customer_information` `j` on((convert(`i`.`relation_id` using utf8) = `j`.`customer_id`))) left join `customer_ledger` `q` on((convert(`i`.`transaction_id` using utf8) = `q`.`transaction_id`)))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `view_person_transection`
--

/*!50001 DROP TABLE IF EXISTS `view_person_transection`*/;
/*!50001 DROP VIEW IF EXISTS `view_person_transection`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = latin1 */;
/*!50001 SET character_set_results     = latin1 */;
/*!50001 SET collation_connection      = latin1_swedish_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `view_person_transection` AS (select `i`.`transaction_id` AS `transaction_id`,`j`.`person_name` AS `person_name` from (`transection` `i` left join `person_information` `j` on((convert(`i`.`relation_id` using utf8) = `j`.`person_id`)))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `view_supplier_transection`
--

/*!50001 DROP TABLE IF EXISTS `view_supplier_transection`*/;
/*!50001 DROP VIEW IF EXISTS `view_supplier_transection`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = latin1 */;
/*!50001 SET character_set_results     = latin1 */;
/*!50001 SET collation_connection      = latin1_swedish_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `view_supplier_transection` AS (select `i`.`transaction_id` AS `transaction_id`,`j`.`supplier_name` AS `supplier_name` from (`transection` `i` left join `supplier_information` `j` on((convert(`i`.`relation_id` using utf8) = `j`.`supplier_id`)))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `view_transection`
--

/*!50001 DROP TABLE IF EXISTS `view_transection`*/;
/*!50001 DROP VIEW IF EXISTS `view_transection`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = latin1 */;
/*!50001 SET character_set_results     = latin1 */;
/*!50001 SET collation_connection      = latin1_swedish_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `view_transection` AS (select `i`.`transaction_id` AS `transaction_id`,`i`.`date_of_transection` AS `date_of_transection`,`i`.`amount` AS `amount`,`i`.`pay_amount` AS `pay_amount`,`f`.`invoice_no` AS `invoice_no`,`g`.`customer_name` AS `customer_name`,`h`.`supplier_name` AS `supplier_name`,`j`.`person_name` AS `person_name`,`i`.`transection_category` AS `transection_category`,`i`.`transection_type` AS `transection_type`,`i`.`transection_mood` AS `transection_mood`,`i`.`description` AS `description`,`i`.`relation_id` AS `relation_id` from ((((`transection` `i` left join `customer_ledger` `f` on((convert(`i`.`transaction_id` using utf8) = `f`.`transaction_id`))) left join `customer_information` `g` on((convert(`i`.`relation_id` using utf8) = `f`.`customer_id`))) left join `supplier_information` `h` on((convert(`i`.`relation_id` using utf8) = `h`.`supplier_id`))) left join `person_information` `j` on((convert(`i`.`relation_id` using utf8) = `j`.`person_id`)))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-03-06  3:09:29
