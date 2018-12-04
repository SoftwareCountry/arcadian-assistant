import React, { Component } from 'react';
import { View, TouchableOpacity, Platform, Dimensions } from 'react-native';
import { connect } from 'react-redux';
import tabBarStyles from '../tabbar/tab-bar-styles';
import { ApplicationIcon } from '../override/application-icon';
import { preferencesStyles } from './preferences.styles';
import { openUserPreferencesAction } from './user-preferences-dispatcher';
import { Action, Dispatch } from 'redux';

interface SettingsDispatchProps {
    onSettingsClicked: () => void;
}

const mapDispatchToProps = (dispatch: Dispatch<Action>): SettingsDispatchProps => ({
    onSettingsClicked: () => {
        dispatch(openUserPreferencesAction());
    }
});

class SettingsViewImpl extends Component<SettingsDispatchProps> {
    public render() {
        return <TouchableOpacity onPress={this.props.onSettingsClicked}>
            <View style={preferencesStyles.settingsView}>
                <ApplicationIcon
                    name={'settings'}
                    size={Platform.OS === 'ios' ? Dimensions.get('window').width * 0.06 : Dimensions.get('window').width * 0.03}
                    style={tabBarStyles.tabImages}/>
            </View>
        </TouchableOpacity>;
    }
}

export const SettingsView = connect(null, mapDispatchToProps)(SettingsViewImpl);
