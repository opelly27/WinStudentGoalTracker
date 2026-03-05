CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`%` SQL SECURITY DEFINER VIEW `winstudentgoaltracker`.`v_progress_event_card` AS
select `pe`.`id_progress_event` AS `progressEventId`,`pe`.`id_goal` AS `goalId`,`g`.`id_student` AS `studentId`,`pe`.`content` AS `content`,`pe`.`created_at` AS `createdAt`,`u`.`name` AS `createdByName`
from ((`winstudentgoaltracker`.`progress_event` `pe`
join `winstudentgoaltracker`.`goal` `g` on((`g`.`id_goal` = `pe`.`id_goal`)))
left join `winstudentgoaltracker`.`user` `u` on((`u`.`id_user` = `pe`.`id_user_created`)));
