#0921�޸�
ALTER TABLE `base_station_management`.`base_station_info` 
CHANGE COLUMN `duration` `duration` DECIMAL(6,2) NULL DEFAULT NULL COMMENT 'Ԥ��ʱ������λСʱ' ;

ALTER TABLE `base_station_management`.`base_station_info` 
CHANGE COLUMN `duration_status` `duration_status` INT(11) NULL DEFAULT '1' COMMENT '1��2��3��4��' ;

ALTER TABLE `gprs_config_info`
ADD COLUMN `max_discharge_cur`  decimal(6,3) NULL DEFAULT 100.000 COMMENT '��Ч�ŵ�������ֵ' AFTER `margin_time`;

#109�޸�
ALTER TABLE `gprs_config_info`
ADD COLUMN `gprs_flag`  int(11) NULL DEFAULT 0 COMMENT '����������ʶ 1 ���� 0 ������';

ALTER TABLE `sub_devices` 
add COLUMN  `sub_flag` int(11) NULL DEFAULT 0 COMMENT '���ôӻ���ʶ 1 ���� 0 ������';

#10/10
ALTER TABLE `gprs_config_info`
ADD COLUMN `gprs_port` varchar(16) NULL DEFAULT null COMMENT '�����˿�';

ALTER TABLE `gprs_config_info`
ADD COLUMN `gprs_spec` varchar(16) NULL DEFAULT null COMMENT '�������';

ALTER TABLE `sub_devices`
ADD COLUMN `sub_spec` varchar(16) NULL DEFAULT null COMMENT '�ӻ��˿�';

ALTER TABLE `sub_devices`
ADD COLUMN `sub_type` int(11) NULL DEFAULT null COMMENT '�豸���ͣ�1���ش��������豸,2���ش�������������';

#10/16
ALTER TABLE `base_station_info`
ADD COLUMN `inspect_status`  int(11) NULL DEFAULT 0 COMMENT '��װ��ά�����̣������״̬�� 0:δ��װ��1:�Ѱ�װ��2:��װ�У�21:��װ�еȴ�ȷ��״̬��22:��װ�к�̨ȷ��δ���״̬��3:ά���У�31:ά���еȴ�ȷ��״̬��32:ά���к�̨ȷ��δ���״̬' AFTER `load_power`;

ALTER TABLE `routing_inspection_detail`
DROP COLUMN `detail_operator_type`;

ALTER TABLE `routing_inspections`
ADD COLUMN `confirm_operate_id`  int(11) NULL DEFAULT NULL COMMENT '��̨ȷ����ԱID' AFTER `gprs_id_device`,
ADD COLUMN `confirm_operate_name`  varchar(32) NULL DEFAULT NULL COMMENT '��̨ȷ����Աname' AFTER `confirm_operate_id`;
#10/20
ALTER TABLE `routing_inspection_detail`
ADD COLUMN `request_seq`  int(11) NULL DEFAULT null COMMENT 'app�˵�һ��������1���ڶ���������2,�Դ�����';

ALTER TABLE `routing_inspection_detail`
ADD COLUMN `request_type`  int(11) NULL DEFAULT 0 COMMENT 'web�˻�Ӧ״̬0��û�л�Ӧ��1 �ǻ�Ӧ��Ĭ����0';
#10/26
ALTER TABLE `pulse_discharge_info`
ADD COLUMN `filter_voltage`  varchar(10000) NULL DEFAULT null COMMENT '���˺�ĵ�ѹ';

ALTER TABLE `pulse_discharge_info`
ADD COLUMN `filter_current`  varchar(10000) NULL DEFAULT null COMMENT '���˺�ĵ���';

#10/31
ALTER TABLE `roles`
MODIFY COLUMN `role_id`  int(11) NOT NULL AUTO_INCREMENT FIRST ;

#11/1
CREATE TABLE `role_permission` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `role_id` int(11) NOT NULL,
  `permission_id` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `user_role` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `role_id` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

#11/6
ALTER TABLE `base_station_info`
ADD COLUMN `address_coding`  varchar(32) NULL DEFAULT null COMMENT 'վַ���';
#11/7
ALTER TABLE `routing_inspections`
ADD COLUMN `operate_phone`  varchar(32) NULL DEFAULT null COMMENT '������Ա�绰'

#11/7
CREATE 
    ALGORITHM = UNDEFINED 
    DEFINER = `amplifi`@`%` 
    SQL SECURITY DEFINER
VIEW `full_station_info` AS
    SELECT 
        `st`.`id` AS `id`,
        `st`.`gprs_id` AS `gprs_id`,
        `st`.`gprs_id_out` AS `gprs_id_out`,
        `st`.`name` AS `name`,
        `st`.`address` AS `address`,
        `st`.`province` AS `province`,
        `st`.`city` AS `city`,
        `st`.`district` AS `district`,
        `st`.`lat` AS `lat`,
        `st`.`lng` AS `lng`,
        `st`.`maintainance_id` AS `maintainance_id`,
        `st`.`pack_type` AS `pack_type`,
        `st`.`room_type` AS `room_type`,
        `st`.`duration` AS `duration`,
        `st`.`real_duration` AS `real_duration`,
        `st`.`ok_num` AS `ok_num`,
        `st`.`poor_num` AS `poor_num`,
        `st`.`error_num` AS `error_num`,
        `st`.`status` AS `status`,
        `st`.`company_id1` AS `company_id1`,
        `st`.`company_id2` AS `company_id2`,
        `st`.`company_id3` AS `company_id3`,
        `st`.`del_flag` AS `del_flag`,
        `st`.`company_name3` AS `company_name3`,
        `st`.`vol_level` AS `vol_level`,
        `st`.`operator_type` AS `operator_type`,
        `st`.`duration_status` AS `duration_status`,
        `st`.`update_time` AS `update_time`,
        `st`.`load_power` AS `load_power`,
        `st`.`inspect_status` AS `inspect_status`,
        `st`.`address_coding` AS `address_coding`,
        `config`.`link_status` AS `link_status`,
        `config`.`device_type` AS `device_type`,
        `pack`.`state` AS `state`,
        `pack`.`gen_vol` AS `gen_vol`,
        `pack`.`gen_cur` AS `gen_cur`
    FROM
        ((`base_station_info` `st`
        LEFT JOIN `gprs_config_info` `config` ON ((`st`.`gprs_id` = `config`.`gprs_id`)))
        LEFT JOIN `pack_data_info_latest` `pack` ON ((`config`.`gprs_id` = `pack`.`gprs_id`)))

#//21
insert into city_code (city_code,city_name,parent_code,TYPE) 
values ('510125','ֱϽ��','442000',4),('510118','����','442000',4),('510119','����','442000',4),('510120','����','442000',4),('510123','����','442000',4)

insert into city_code (city_code,city_name,parent_code,TYPE) 
values ('510126','ֱϽ��','441900',4)

#11/22
ALTER TABLE `routing_inspections`
ADD COLUMN `remark`  varchar(255) NULL COMMENT 'Ѳ���¼��ע' AFTER `operate_phone`;

#11/23
CREATE TABLE `base_station_management`.`device_discharge_autocheck` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `gprs_id` VARCHAR(12) NULL,
  `first_discharge_time` DATETIME NULL COMMENT '��һ�ηŵ�ʱ��',
  `start_vol` DECIMAL(6,2) NULL COMMENT '��ʼ��ѹ',
  `end_vol` DECIMAL(6,2) NULL COMMENT '������ѹ',
  `is_correct` INT NULL COMMENT '����״̬�Ƿ���ȷ���ŵ��ѹ���ͼ�Ϊ��ȷ��',
  `check_date` DATETIME NULL COMMENT '���ʱ�䣬д��ü�¼��ʱ��',
  `correct_date` DATETIME NULL COMMENT '�豸״̬�޸���ʱ��',
  PRIMARY KEY (`id`))
COMMENT = '�豸�ŵ�����״̬�жϱ�';

#11/23
ALTER TABLE `device_discharge_autocheck`
ADD COLUMN `data_updated`  int(11) NULL DEFAULT 0 COMMENT '�����Ƿ����޸� 0 δ�޸���1 �޸�';

#11/28
ALTER TABLE `routing_inspections`
ADD COLUMN `gprs_device_type`  varchar(32) NULL DEFAULT null COMMENT '�豸����';

#11/29
ALTER TABLE `users`
DROP INDEX `user_phone_UNIQUE`;

#11/29
alter table routing_inspections change gprs_device_type device_type varchar(32) DEFAULT NULL
alter table routing_inspections change gprs_id_device gprs_id varchar(12) DEFAULT NULL

#12/5
ALTER TABLE `parameters`
MODIFY COLUMN `parameter_value`  varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NULL DEFAULT NULL AFTER `parameter_code`;

#12/8
delete from city_code where city_name  in ('中沙群岛的岛礁及其海域' , '西沙群岛', '南沙群岛')
update city_code set city_name ='海南省直辖行政单位' where city_code = '469000'
update city_code set city_name ='湖北省直辖行政单位' where city_code = '429000'

 #12/14
 ALTER TABLE `gprs_config_info`
ADD COLUMN `charge_interval`  int(11) NULL DEFAULT '60' COMMENT '充电状态下状态帧传输间隔，单位秒';

ALTER TABLE `gprs_config_send`
ADD COLUMN `charge_interval`  int(11) NULL DEFAULT '60' COMMENT '充电状态下状态帧传输间隔，单位秒';
#12/17
ALTER TABLE `users`
ADD COLUMN `user_code`  int(11) NULL DEFAULT null COMMENT '用户验证码';
#12/20
alter table gprs_balance_send add column mode int(11) null default null comment '1：强制执行，从机均衡状态由后台控制； 0：非强制，从机可以启用自动均衡'

#12/25
alter table base_station_info 
add column cell_count int(11) null default null comment '电池总数';

alter table gprs_config_info 
add column sub_device_count int(11) null default null comment '从机总数';

 CREATE TABLE `cell_vol_level` (                                                                         
                  `id` int(11) NOT NULL AUTO_INCREMENT,                                                                 
                  `vol_level_name` varchar(32) DEFAULT NULL COMMENT '电压平台名称',                               
                  `vol_level_code` int(11) DEFAULT NULL COMMENT '电压平台编码',                                   
                  `create_id` varchar(32) DEFAULT NULL COMMENT '登录人员id',                                               
                  `create_time` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '创建时间',  
                  PRIMARY KEY (`id`) USING BTREE                                                                        
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8  
  
 CREATE TABLE `gprs_device_type` (                                                                       
                    `id` int(11) NOT NULL AUTO_INCREMENT,                                                                 
                    `type_code` int(11) DEFAULT NULL COMMENT '设备类型编号',                                        
                    `type_name` varchar(32) DEFAULT NULL COMMENT '设备类型',                                          
                    `sub_vol` decimal(5,3) unsigned DEFAULT NULL COMMENT '从机电压',                                  
                    `vol_level` int(11) DEFAULT NULL COMMENT '电压平台编码 FK',                                     
                    `create_id` varchar(32) DEFAULT NULL COMMENT '登录人员id',                                         
                    `create_time` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '创建时间',  
                    PRIMARY KEY (`id`)                                                                                    
                  ) ENGINE=InnoDB DEFAULT CHARSET=utf8   
                  

 #12/27                 
ALTER TABLE `base_station_management`.`parameters` 
CHANGE COLUMN `parameter_category` `parameter_category` VARCHAR(32) CHARACTER SET 'utf8' NOT NULL COMMENT '参数类别，用以区分同一个参数名，不同的设备或者分公司。' AFTER `parameter_code`,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`parameter_code`, `parameter_category`);

ALTER TABLE `gprs_device_type`
ADD COLUMN sub_device_count  int(11) NULL DEFAULT null COMMENT '从机数量';  

#12/29
ALTER TABLE base_station_info alter column inspect_status drop default;
ALTER TABLE base_station_info alter column inspect_status set default 99; 
#2018/1/3

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for gprs_device_type
-- ----------------------------
DROP TABLE IF EXISTS `gprs_device_type`;
CREATE TABLE `gprs_device_type` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type_code` int(11) DEFAULT NULL COMMENT '设备类型编号',
  `type_name` varchar(32) DEFAULT NULL COMMENT '设备类型 ，1蓄电池串联复用设备,2蓄电池串联复用诊断组件，3，蓄电池4V监测设备，4，蓄电池12V监测设备',
  `sub_vol` decimal(5,3) unsigned DEFAULT NULL COMMENT '从机电压',
  `vol_level` int(11) DEFAULT NULL COMMENT '电压平台编码 FK',
  `create_id` varchar(32) DEFAULT NULL COMMENT '登录人员id',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '创建时间',
  `sub_device_count` int(11) DEFAULT NULL COMMENT '从机数量',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of gprs_device_type
-- ----------------------------
INSERT INTO `gprs_device_type` VALUES ('1', '1', '蓄电池串联复用设备', '4.000', '2', 'admin', '2017-12-27 14:43:24', '24');
INSERT INTO `gprs_device_type` VALUES ('2', '2', '蓄电池串联复用诊断组件', '4.000', '2', 'admin', '2017-12-27 14:44:21', '24');
INSERT INTO `gprs_device_type` VALUES ('3', '3', '蓄电池2V监测设备', '4.000', '2', 'admin', '2017-12-27 14:45:31', '24');
INSERT INTO `gprs_device_type` VALUES ('4', '4', '蓄电池12V监测设备', '4.000', '12', 'admin', '2017-12-27 14:45:59', '4');  

#2018/1/3
INSERT INTO `permissions` (`permission_type`, `permission_name`, `permission_code`, `parent_id`, `permission_system`) VALUES ('1', '单体电压平台管理', '10003', '1', '1') ;
INSERT INTO `permissions` (`permission_type`, `permission_name`, `permission_code`, `parent_id`, `permission_system`) VALUES ('1', '设备类型管理', '10004', '1', '1') ;  

#2018/1/5
SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for cell_vol_level
-- ----------------------------
DROP TABLE IF EXISTS `cell_vol_level`;
CREATE TABLE `cell_vol_level` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `vol_level_name` varchar(32) DEFAULT NULL COMMENT '电压平台名称',
  `vol_level_code` int(11) DEFAULT NULL COMMENT '电压平台编码',
  `create_id` varchar(32) DEFAULT NULL COMMENT '登录人员id',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of cell_vol_level
-- ----------------------------
INSERT INTO `cell_vol_level` VALUES ('1', '12', '12', 'admin', '2017-12-29 14:01:50');
INSERT INTO `cell_vol_level` VALUES ('2', '2', '2', 'admin', '2017-12-29 14:02:25');

#2018/18
CREATE INDEX rcv_time ON warning_info (gprs_id,rcv_time)


#2018/1/9
ALTER 
ALGORITHM=UNDEFINED 
DEFINER=`amplifi`@`%` 
SQL SECURITY DEFINER 
VIEW `full_station_info` AS 
SELECT
	`st`.`id` AS `id`,
	`st`.`gprs_id` AS `gprs_id`,
	`st`.`gprs_id_out` AS `gprs_id_out`,
	`st`.`name` AS `name`,
	`st`.`address` AS `address`,
	`st`.`province` AS `province`,
	`st`.`city` AS `city`,
	`st`.`district` AS `district`,
	`st`.`lat` AS `lat`,
	`st`.`lng` AS `lng`,
	`st`.`maintainance_id` AS `maintainance_id`,
	`st`.`pack_type` AS `pack_type`,
	`st`.`room_type` AS `room_type`,
	`st`.`duration` AS `duration`,
	`st`.`real_duration` AS `real_duration`,
	`st`.`ok_num` AS `ok_num`,
	`st`.`poor_num` AS `poor_num`,
	`st`.`error_num` AS `error_num`,
	`st`.`status` AS `status`,
	`st`.`company_id1` AS `company_id1`,
	`st`.`company_id2` AS `company_id2`,
	`st`.`company_id3` AS `company_id3`,
	`st`.`del_flag` AS `del_flag`,
	`st`.`company_name3` AS `company_name3`,
	`st`.`vol_level` AS `vol_level`,
	`st`.`operator_type` AS `operator_type`,
	`st`.`duration_status` AS `duration_status`,
	`st`.`update_time` AS `update_time`,
	`st`.`load_power` AS `load_power`,
	`st`.`inspect_status` AS `inspect_status`,
	`st`.`address_coding` AS `address_coding`,
	`st`.`cell_count` AS `cell_count`,
	`config`.`link_status` AS `link_status`,
	`config`.`device_type` AS `device_type`,
	`pack`.`state` AS `state`,
	`pack`.`gen_vol` AS `gen_vol`,
	`pack`.`gen_cur` AS `gen_cur`
FROM
	(
		(
			`base_station_info` `st`
			LEFT JOIN `gprs_config_info` `config` ON (
				(
					`st`.`gprs_id` = `config`.`gprs_id`
				)
			)
		)
		LEFT JOIN `pack_data_info_latest` `pack` ON (
			(
				`config`.`gprs_id` = `pack`.`gprs_id`
			)
		)
	) ;

#1/10
ALTER  TABLE  pack_data_info ALGORITHM=inplace, LOCK=NONE, DROP INDEX `rcv_index`,
ADD INDEX `rcv_index` (`rcv_time` ASC, `gprs_id` ASC, `state` ASC);          