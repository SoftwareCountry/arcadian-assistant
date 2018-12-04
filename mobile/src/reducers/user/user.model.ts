import { dataMember, required } from 'santee-dcts';

export class User {
    @dataMember()
    @required()
    public username: string = '';

    @dataMember()
    @required()
    public employeeId: string = '';
}
