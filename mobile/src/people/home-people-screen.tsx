import { StackNavigator } from 'react-navigation';
import React from 'react';
import { Text, View, Image, Platform, StatusBar } from 'react-native';
import { TopNavBar } from '../topNavBar/top-nav-bar';
import { TopTabBarNavigator } from '../toptabbar/top-tab-bar-navigator';
import { Department } from '../reducers/organization/department.model';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';

interface PeopleScreenProps {
    departments: Department[];
}

const mapStateToProps = (state: AppState): PeopleScreenProps => ({
    departments: state.organization.departments
});

const navBar =  new TopNavBar('People');

class HomePeopleScreenImpl extends React.Component<PeopleScreenProps> {
    public static navigationOptions = navBar.configurate();
    
    public render() {
        
        return (
            <View style={{ height: 500, flex: 1 }}>
            <TopTabBarNavigator />
            </View>
        );
    }
}

export const HomePeopleScreen = connect(mapStateToProps)(HomePeopleScreenImpl);