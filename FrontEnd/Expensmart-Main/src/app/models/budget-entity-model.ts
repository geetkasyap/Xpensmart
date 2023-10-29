export interface BudgetsEntity {
    budgetID:number;
    userID:number;
    amount:number;
    startDate:Date;
    endDate:Date;
    isActive:boolean;
    frequency:string;
    createdOn:Date;
    modifiedOn:Date;
    CreatedBy:string;
    ModifiedBy:string;
    UsersEntity:any;
    }