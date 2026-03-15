CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`%` SQL SECURITY DEFINER VIEW `winstudentgoaltracker`.`v_goal_card` AS
select `g`.`id_goal` AS `goalId`,`g`.`id_goal_parent` AS `goalParentId`,`g`.`id_student` AS `studentId`,`g`.`description` AS `description`,`g`.`category` AS `category`,`g`.`baseline` AS `baseline`,`g`.`target_completion_date` AS `targetCompletionDate`,`g`.`close_date` AS `closeDate`,`g`.`achieved` AS `achieved`,`g`.`close_notes` AS `closeNotes`,count(distinct `pe`.`id_progress_event`) AS `progressEventCount`,count(distinct `b`.`id_benchmark`) AS `benchmarkCount`
from ((`winstudentgoaltracker`.`goal` `g`
left
join `winstudentgoaltracker`.`progress_event` `pe` on((`pe`.`id_goal` = `g`.`id_goal`)))
left
join `winstudentgoaltracker`.`benchmark` `b` on((`b`.`id_goal` = `g`.`id_goal`))) group by `g`.`id_goal`,`g`.`id_goal_parent`,`g`.`id_student`,`g`.`description`,`g`.`category`,`g`.`baseline`,`g`.`target_completion_date`,`g`.`close_date`,`g`.`achieved`,`g`.`close_notes`;
