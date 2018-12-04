import { dataMember, required } from 'santee-dcts';

export class UserPreferences {

    public static defaultUserPreferences = new UserPreferences();

    @dataMember({ fieldName: 'emailNotifications' })
    @required()
    public areEmailNotificationsEnabled: boolean;

    @dataMember({ fieldName: 'pushNotifications' })
    @required()
    public arePushNotificationsEnabled: boolean;

    public constructor() {
        this.areEmailNotificationsEnabled = false;
        this.arePushNotificationsEnabled = false;
    }

    public clone(): UserPreferences {
        const result = new UserPreferences();

        result.areEmailNotificationsEnabled = this.areEmailNotificationsEnabled;
        result.arePushNotificationsEnabled = this.arePushNotificationsEnabled;

        return result;
    }

    public equals(obj: UserPreferences): boolean {
        if (!obj) {
            return false;
        }

        if (obj === this) {
            return true;
        }

        return this.areEmailNotificationsEnabled === obj.areEmailNotificationsEnabled &&
            this.arePushNotificationsEnabled === obj.arePushNotificationsEnabled;
    }
}

