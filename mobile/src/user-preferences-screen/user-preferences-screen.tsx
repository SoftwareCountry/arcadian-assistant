import * as React from 'react';
import { Component } from 'react';
import { RefreshControl, SafeAreaView, ScrollView } from 'react-native';
import { connect } from 'react-redux';
import { loadUserPreferences, updateUserPreferences, UserActions } from '../reducers/user/user.action';
import { UserPreferences } from '../reducers/user/user-preferences.model';
import { AppState } from '../reducers/app.reducer';
import { LoadingView } from '../navigation/loading';
import { preferencesStyles } from './preferences.styles';
import { SwitchSettingsView } from './switch-setting-view';
import { NavigationScreenConfig, NavigationStackScreenOptions } from 'react-navigation';
import { Dispatch } from 'redux';
import { Nullable } from 'types';

interface UserPreferencesScreenProps {
    userId: Nullable<string>;
    preferences: Nullable<UserPreferences>;
}

const mapStateToProps = (state: AppState): UserPreferencesScreenProps => ({
    userId: state.userInfo ? state.userInfo.employeeId : null,
    preferences: state.userInfo ? state.userInfo.preferences : null,
});

interface UserPreferencesDispatchProps {
    loadUserPreferences: (userId: string) => void;
    updateUserPreferences: (userId: string, previousPreferences: UserPreferences, newPreferences: UserPreferences) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<UserActions>): UserPreferencesDispatchProps => ({
    loadUserPreferences: (userId: string) => {
        dispatch(loadUserPreferences(userId));
    },
    updateUserPreferences: (userId: string, previousPreferences: UserPreferences, newPreferences: UserPreferences) => {
        dispatch(updateUserPreferences(userId, previousPreferences, newPreferences));
    }
});

class UserPreferencesScreenImpl extends Component<UserPreferencesScreenProps & UserPreferencesDispatchProps> {

    public static navigationOptions: NavigationScreenConfig<NavigationStackScreenOptions> = {
        headerTitle: 'Preferences',
        headerTitleStyle: preferencesStyles.headerTitle,
    };

    public componentDidMount() {
        this.loadPreferences();
    }

    public render() {
        const { userId, preferences } = this.props;

        if (!userId || !preferences) {
            return <LoadingView/>;
        }

        return <SafeAreaView style={preferencesStyles.container}>
            <ScrollView refreshControl={<RefreshControl refreshing={false} onRefresh={this.loadPreferences}/>}>
                <SwitchSettingsView title='Email notifications'
                                    onValueChange={this.onEnableEmailNotificationsChanged}
                                    value={preferences.emailNotifications}/>
                <SwitchSettingsView title='Push notifications' onValueChange={this.onEnablePushNotificationsChanged}
                                    value={preferences.pushNotifications}/>
            </ScrollView>
        </SafeAreaView>;
    }

    private loadPreferences = () => {
        if (this.props.userId) {
            this.props.loadUserPreferences(this.props.userId);
        }
    };

    private onEnableEmailNotificationsChanged = (value: boolean) => {
        const { userId, preferences } = this.props;

        if (!userId || !preferences) {
            return;
        }

        const newPreferences = preferences.clone();
        newPreferences.emailNotifications = value;

        this.props.updateUserPreferences(userId, preferences, newPreferences);
    };

    private onEnablePushNotificationsChanged = (value: boolean) => {
        const { userId, preferences } = this.props;

        if (!userId || !preferences) {
            return;
        }

        const newPreferences = preferences.clone();
        newPreferences.pushNotifications = value;

        this.props.updateUserPreferences(userId, preferences, newPreferences);
    };
}

export const UserPreferencesScreen = connect(mapStateToProps, mapDispatchToProps)(UserPreferencesScreenImpl);
