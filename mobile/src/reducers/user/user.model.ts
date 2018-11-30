import { dataMember, required } from 'santee-dcts';

export class User {
    @dataMember()
    @required()
    public username = '';

    @dataMember()
    @required()
    public employeeId = '';
}
