<?php
$NEXTQUERY = 
       "SELECT question.id, question.text, question.title 
        FROM client, form, questionnaire, questionlist, question, 
        ( 
            SELECT IFNULL(MAX(answer.qid), 0) AS id 
            FROM client, answer 
            WHERE client.id = answer.clientid 
        ) AS lastanswer 
        WHERE client.id = ? AND client.hash = ? 
        AND client.formid = form.id 
        AND form.airid = questionnaire.id 
        AND questionlist.airid = questionnaire.id 
        AND questionlist.qid = question.id 
        AND question.id > lastanswer.id 
        ORDER BY question.id ASC 
        LIMIT 1 
        ;";
?>
