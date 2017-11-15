import React, { Component } from 'react';
import { View, Text, ScrollView, Button } from 'react-native';
import { NavigationScreenProps, NavigationActions } from 'react-navigation';
import { AppState } from '../reducers/app.reducer';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { WithBackButtonProps, mapBackButtonDispatchToProps } from '../layout/back-button-dispatcher';

interface HelpdeskScreenProps {
    messageTemplates: {}[];
}

const mapStateToProps: MapStateToProps<HelpdeskScreenProps, any> = (state: AppState) => ({
    messageTemplates: state.helpdesk.messageTemplates
});

const mapDispatchToProps: MapDispatchToPropsFunction<WithBackButtonProps, any> = (dispatch: Dispatch<any>) => ({
    ...mapBackButtonDispatchToProps(dispatch)
});

class HelpdeskScreenImpl extends Component<HelpdeskScreenProps & WithBackButtonProps> {

    public render() {

        console.log(this.props);

        return <ScrollView>
                <Text>Si!</Text>
                <Button title='Back' onPress={ () => this.props.onBackClick() } ></Button>
            </ScrollView>;
    }
}

//export const ActionsScreen = ActionsScreenImpl;

export const HelpdeskScreen = connect(mapStateToProps, mapDispatchToProps)(HelpdeskScreenImpl);