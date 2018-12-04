import { NavigationActions } from 'react-navigation';

export const openUserPreferencesAction = () => NavigationActions.navigate({
    routeName: 'UserPreferences',
});
