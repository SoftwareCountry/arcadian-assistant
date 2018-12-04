import React, { Component } from 'react';
import { View, Text, ScrollView, Button, ActivityIndicator } from 'react-native';
import { NavigationScreenProps, NavigationActions } from 'react-navigation';
import { AppState } from '../reducers/app.reducer';
import { connect, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { WithBackButtonProps, mapBackButtonDispatchToProps } from '../layout/back-button-dispatcher';
import { TicketTemplate } from '../reducers/helpdesk/ticket-template.model';
import { loadTicketTemplates } from '../reducers/helpdesk/tickets.actions';
import { Action, Dispatch } from 'redux';

interface HelpdeskScreenProps {
    ticketTemplatesAreLoaded: boolean;
}

interface HelpdeskScreenDispatchProps extends WithBackButtonProps {
    requestTicketTemplates: () => void;
}

const mapStateToProps = (state: AppState): HelpdeskScreenProps => ({
    ticketTemplatesAreLoaded: !!state.helpdesk && !!state.helpdesk.ticketTemplates,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>): HelpdeskScreenDispatchProps => ({
    ...mapBackButtonDispatchToProps(dispatch),
    requestTicketTemplates: () => dispatch(loadTicketTemplates()),
});

class HelpdeskScreenImpl extends Component<HelpdeskScreenProps & HelpdeskScreenDispatchProps> {
    public componentDidMount() {
        this.props.requestTicketTemplates();
    }

    public render() {

        const progressBar = this.props.ticketTemplatesAreLoaded ? undefined : <ActivityIndicator/>;

        return <ScrollView>
                {progressBar}
                <Button title='Back' onPress={ () => this.props.onBackClick() } />
            </ScrollView>;
    }
}

export const HelpdeskScreen = connect(mapStateToProps, mapDispatchToProps)(HelpdeskScreenImpl);
