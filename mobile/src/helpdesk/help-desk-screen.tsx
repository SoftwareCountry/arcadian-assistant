import React, { Component } from 'react';
import { ActivityIndicator, Button, ScrollView } from 'react-native';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { mapBackButtonDispatchToProps, WithBackButtonProps } from '../layout/back-button-dispatcher';
import { loadTicketTemplates } from '../reducers/help-desk/tickets.actions';
import { Action, Dispatch } from 'redux';

interface HelpDeskScreenProps {
    ticketTemplatesAreLoaded: boolean;
}

interface HelpDeskScreenDispatchProps extends WithBackButtonProps {
    requestTicketTemplates: () => void;
}

const mapStateToProps = (state: AppState): HelpDeskScreenProps => ({
    ticketTemplatesAreLoaded: !!state.helpDesk && !!state.helpDesk.ticketTemplates,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>): HelpDeskScreenDispatchProps => ({
    ...mapBackButtonDispatchToProps(dispatch),
    requestTicketTemplates: () => dispatch(loadTicketTemplates()),
});

class HelpDeskScreenImpl extends Component<HelpDeskScreenProps & HelpDeskScreenDispatchProps> {
    public componentDidMount() {
        this.props.requestTicketTemplates();
    }

    public render() {

        const progressBar = this.props.ticketTemplatesAreLoaded ? undefined : <ActivityIndicator/>;

        return <ScrollView>
            {progressBar}
            <Button title='Back' onPress={() => this.props.onBackClick()}/>
        </ScrollView>;
    }
}

export const HelpDeskScreen = connect(mapStateToProps, mapDispatchToProps)(HelpDeskScreenImpl);
