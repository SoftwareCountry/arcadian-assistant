import { dataMember, required } from 'santee-dcts';

export class UserPreferences {

    @dataMember()
    @required()
    public emailNotifications: boolean;

    @dataMember()
    @required()
    public pushNotifications: boolean;

    public constructor() {
        this.emailNotifications = false;
        this.pushNotifications = false;
    }

    public clone(): UserPreferences {
        const result = new UserPreferences();

        result.emailNotifications = this.emailNotifications;
        result.pushNotifications = this.pushNotifications;

        return result;
    }

    public equals(obj: UserPreferences): boolean {
        if (!obj) {
            return false;
        }

        if (obj === this) {
            return true;
        }

        return this.emailNotifications === obj.emailNotifications &&
            this.pushNotifications === obj.pushNotifications;
    }
}

