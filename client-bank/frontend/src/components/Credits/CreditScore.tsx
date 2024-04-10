import {useEffect, useState} from "react";
import {isAuthenticated} from "../../api/auth";
import {CreditScoreDTO, CreditService} from "../../api/credit";

export const CreditScore = () => {
    const [creditScore,setCreditScore]=useState<CreditScoreDTO>();

    useEffect(() => {
        const fetchUserScore=async () => {

            try {
                if (!isAuthenticated())
                    throw new Error("not authenticated")
                let score = await CreditService.getUserCreditScore();
                setCreditScore(score)
                console.log(score.score)
            } catch (e) {
                console.log(e);
            }
        }

        fetchUserScore()

    }, []);

    return (
        <div>
            {creditScore? `Score: ${creditScore.score}`:'Loading credit score'}
        </div>
    );
};