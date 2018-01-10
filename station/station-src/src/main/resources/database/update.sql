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
        `pack`.`state` AS `state`
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