CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`%` SQL SECURITY DEFINER VIEW `winstudentgoaltracker`.`v_student_card` AS
select `s`.`id_student` AS `studentId`,`s`.`identifier` AS `identifier`,`s`.`next_iep_date` AS `nextIepDate`,max(`pe`.`created_at`) AS `lastEntryDate`,count(distinct `g`.`id_goal`) AS `goalCount`,count(distinct `pe`.`id_progress_event`) AS `progressEventCount`
from ((`winstudentgoaltracker`.`student` `s`
left
join `winstudentgoaltracker`.`goal` `g` on((`g`.`id_student` = `s`.`id_student`)))
left
join `winstudentgoaltracker`.`progress_event` `pe` on((`pe`.`id_goal` = `g`.`id_goal`))) group by `s`.`id_student`,`s`.`identifier`,`s`.`next_iep_date`;
