CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`%` SQL SECURITY DEFINER VIEW `winstudentgoaltracker`.`v_goal_card` AS
select `winstudentgoaltracker`.`goal`.`id_goal` AS `goalId`,`winstudentgoaltracker`.`goal`.`id_goal_parent` AS `goalParentId`,`winstudentgoaltracker`.`goal`.`id_student` AS `studentId`,`winstudentgoaltracker`.`goal`.`title` AS `title`,`winstudentgoaltracker`.`goal`.`description` AS `description`,`winstudentgoaltracker`.`goal`.`category` AS `category`,count(`pe`.`id_progress_event`) AS `progressEventCount`
from (`winstudentgoaltracker`.`goal`
left
join `winstudentgoaltracker`.`progress_event` `pe` on((`pe`.`id_goal` = `winstudentgoaltracker`.`goal`.`id_goal`))) group by `winstudentgoaltracker`.`goal`.`id_goal`,`winstudentgoaltracker`.`goal`.`id_goal_parent`,`winstudentgoaltracker`.`goal`.`id_student`,`winstudentgoaltracker`.`goal`.`title`,`winstudentgoaltracker`.`goal`.`description`,`winstudentgoaltracker`.`goal`.`category`;
